FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["AuctionService.Api/AuctionService.Api.csproj", "AuctionService.Api/"]
COPY ["AuctionService.Application/AuctionService.Application.csproj", "AuctionService.Application/"]
COPY ["AuctionService.Infrastructure/AuctionService.Infrastructure.csproj", "AuctionService.Infrastructure/"]
COPY ["AuctionService.Domain/AuctionService.Domain.csproj", "AuctionService.Domain/"]

RUN dotnet restore "AuctionService.Api/AuctionService.Api.csproj"

COPY . .
WORKDIR "/src/AuctionService.Api"
RUN dotnet publish "AuctionService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "AuctionService.Api.dll"]