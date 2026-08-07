using RealTimeService.Api.BackgroundJobs;
using RealtimeService.Api.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddHostedService<LiveBiddingKafkaConsumerBackgroundService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("SignalRPolicy", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowed(_ => true)
              .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("SignalRPolicy");

app.MapHub<AuctionHub>("/auction-hub");

app.Run();