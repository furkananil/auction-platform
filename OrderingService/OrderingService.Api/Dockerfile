FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["OrderingService.Api/OrderingService.Api.csproj", "OrderingService.Api/"]
COPY ["OrderingService.Application/OrderingService.Application.csproj", "OrderingService.Application/"]
COPY ["OrderingService.Infrastructure/OrderingService.Infrastructure.csproj", "OrderingService.Infrastructure/"]
COPY ["OrderingService.Domain/OrderingService.Domain.csproj", "OrderingService.Domain/"]

RUN dotnet restore "OrderingService.Api/OrderingService.Api.csproj"

COPY . .
WORKDIR "/src/OrderingService.Api"
RUN dotnet publish "OrderingService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "OrderingService.Api.dll"]