FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["NotificationDelivery.Worker/NotificationDelivery.Worker.csproj", "NotificationDelivery.Worker/"]
COPY ["Shared/Shared.csproj", "Shared/"]

RUN dotnet restore "NotificationDelivery.Worker/NotificationDelivery.Worker.csproj"

COPY . .
WORKDIR "/src/NotificationDelivery.Worker"
RUN dotnet publish "NotificationDelivery.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "NotificationDelivery.Worker.dll"]