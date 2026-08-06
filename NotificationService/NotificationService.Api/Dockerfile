FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["NotificationService.Api/NotificationService.Api.csproj", "NotificationService.Api/"]
COPY ["Shared/Shared.csproj", "Shared/"]

RUN dotnet restore "NotificationService.Api/NotificationService.Api.csproj"

COPY . .
WORKDIR "/src/NotificationService.Api"
RUN dotnet publish "NotificationService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "NotificationService.Api.dll"]