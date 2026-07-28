FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["CatalogService.Api/CatalogService.Api.csproj", "CatalogService.Api/"]
COPY ["CatalogService.Application/CatalogService.Application.csproj", "CatalogService.Application/"]
COPY ["CatalogService.Infrastructure/CatalogService.Infrastructure.csproj", "CatalogService.Infrastructure/"]
COPY ["CatalogService.Domain/CatalogService.Domain.csproj", "CatalogService.Domain/"]

RUN dotnet restore "CatalogService.Api/CatalogService.Api.csproj"

COPY . .
WORKDIR "/src/CatalogService.Api"
RUN dotnet publish "CatalogService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CatalogService.Api.dll"]