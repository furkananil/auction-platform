FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["BiddingService.Api/BiddingService.Api.csproj", "BiddingService.Api/"]
COPY ["BiddingService.Application/BiddingService.Application.csproj", "BiddingService.Application/"]
COPY ["BiddingService.Infrastructure/BiddingService.Infrastructure.csproj", "BiddingService.Infrastructure/"]
COPY ["BiddingService.Domain/BiddingService.Domain.csproj", "BiddingService.Domain/"]

RUN dotnet restore "BiddingService.Api/BiddingService.Api.csproj"

COPY . .
WORKDIR "/src/BiddingService.Api"
RUN dotnet publish "BiddingService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BiddingService.Api.dll"]