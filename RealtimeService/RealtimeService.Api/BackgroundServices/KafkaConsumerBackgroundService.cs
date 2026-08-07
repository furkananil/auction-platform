namespace RealTimeService.Api.BackgroundJobs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.SignalR;
using Confluent.Kafka;
using System.Text.Json;
using RealtimeService.Api.Hubs;
using RealtimeService.Api.Models;

public class LiveBiddingKafkaConsumerBackgroundService : BackgroundService
{
    private readonly IHubContext<AuctionHub> _hubContext;
    private readonly ConsumerConfig _consumerConfig;
    private const string TopicName = "bidding-events";

    public LiveBiddingKafkaConsumerBackgroundService(IHubContext<AuctionHub> hubContext, IConfiguration configuration)
    {
        _hubContext = hubContext;
        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            GroupId = "realtime-service-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build();
        consumer.Subscribe(TopicName);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = consumer.Consume(stoppingToken);
                if (consumeResult == null) continue;

                var bidEvent = JsonSerializer.Deserialize<BidPlacedIntegrationEvent>(consumeResult.Message.Value);
                
                if (bidEvent != null)
                {
                    await _hubContext.Clients
                        .Group(bidEvent.AuctionId.ToString())
                        .SendAsync("ReceiveNewBid", bidEvent, cancellationToken: stoppingToken);
                }
            }
            catch (Exception)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        consumer.Close();
    }
}