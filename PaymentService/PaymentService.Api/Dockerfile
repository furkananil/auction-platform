FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["PaymentService.Api/PaymentService.Api.csproj", "PaymentService.Api/"]
COPY ["PaymentService.Application/PaymentService.Application.csproj", "PaymentService.Application/"]
COPY ["PaymentService.Infrastructure/PaymentService.Infrastructure.csproj", "PaymentService.Infrastructure/"]
COPY ["PaymentService.Domain/PaymentService.Domain.csproj", "PaymentService.Domain/"]

RUN dotnet restore "PaymentService.Api/PaymentService.Api.csproj"

COPY . .
WORKDIR "/src/PaymentService.Api"
RUN dotnet publish "PaymentService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "PaymentService.Api.dll"]