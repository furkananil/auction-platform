namespace RealtimeService.Api.BackgroundServices;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using Confluent.Kafka;
using System.Text.Json;
using RealtimeService.Api.Hubs;
using RealtimeService.Api.Models;

public class KafkaConsumerBackgroundService : BackgroundService
{
    private readonly IHubContext<AuctionHub> _hubContext;
    private readonly ILogger<KafkaConsumerBackgroundService> _logger;
    private readonly ConsumerConfig _consumerConfig;
    private const string TopicName = "bidding-events";

    public KafkaConsumerBackgroundService(
        IHubContext<AuctionHub> hubContext,
        IConfiguration configuration,
        ILogger<KafkaConsumerBackgroundService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;

        var bootstrapServers = configuration["Kafka:BootstrapServers"];
        if (string.IsNullOrWhiteSpace(bootstrapServers))
        {
            bootstrapServers = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true"
                ? "kafka:9093"
                : "localhost:9092";
        }

        _logger.LogInformation("Kafka consumer bootstrap server: {BootstrapServers}", bootstrapServers);

        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
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
                if (consumeResult != null)
                {
                    var bidEvent = JsonSerializer.Deserialize<BidPlacedEvent>(consumeResult.Message.Value);

                    if (bidEvent != null)
                    {
                        await _hubContext.Clients
                            .Group(bidEvent.AuctionId.ToString())
                            .SendAsync("ReceiveNewBid", bidEvent, cancellationToken: stoppingToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kafka consumer loop failed while reading topic {TopicName}", TopicName);
                await Task.Delay(1000, stoppingToken);
            }
        }

        consumer.Close();
    }
}