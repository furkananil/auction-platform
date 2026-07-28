using RealtimeService.Api.BackgroundServices;
using RealtimeService.Api.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policyBuilder =>
    {
        var allowedOrigins = new[]
        {
            "http://127.0.0.1:5500",
            "http://localhost:5500",
            "http://127.0.0.1:5102",
            "http://localhost:5102"
        };

        policyBuilder.SetIsOriginAllowed(origin =>
               string.IsNullOrWhiteSpace(origin) ||
               string.Equals(origin, "null", StringComparison.OrdinalIgnoreCase) ||
               allowedOrigins.Contains(origin))
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials(); // SignalR kimlik doğrulaması/negotiation için zorunlu!
    });
});

builder.Services.AddHostedService<KafkaConsumerBackgroundService>();

var app = builder.Build();

var aspNetCoreUrls = builder.Configuration["ASPNETCORE_URLS"];
var hasHttpsBinding = !string.IsNullOrWhiteSpace(aspNetCoreUrls) &&
    aspNetCoreUrls.Contains("https://", StringComparison.OrdinalIgnoreCase);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("CorsPolicy"); 
app.MapHub<AuctionHub>("/auctionhub");

if (hasHttpsBinding)
{
    app.UseHttpsRedirection();
}

app.Run();
