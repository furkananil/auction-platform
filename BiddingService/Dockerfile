FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["BiddingService/BiddingService.Api/BiddingService.Api.csproj", "BiddingService/BiddingService.Api/"]
COPY ["BiddingService/BiddingService.Application/BiddingService.Application.csproj", "BiddingService/BiddingService.Application/"]
COPY ["BiddingService/BiddingService.Infrastructure/BiddingService.Infrastructure.csproj", "BiddingService/BiddingService.Infrastructure/"]
COPY ["BiddingService/BiddingService.Domain/BiddingService.Domain.csproj", "BiddingService/BiddingService.Domain/"]
COPY ["Shared/Shared.csproj", "Shared/"]

RUN dotnet restore "BiddingService/BiddingService.Api/BiddingService.Api.csproj"

COPY . .
WORKDIR "/src/BiddingService/BiddingService.Api"
RUN dotnet publish "BiddingService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BiddingService.Api.dll"]