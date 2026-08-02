namespace AuctionService.Infrastructure.BackgroundServices;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Confluent.Kafka;
using AuctionService.Infrastructure.Persistence;

public class AuctionOutboxPublisherBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ProducerConfig _producerConfig;
    private const string TopicName = "auction-events";

    public AuctionOutboxPublisherBackgroundService(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _producerConfig = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"]
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();

                var messages = await dbContext.OutboxMessages
                    .Where(m => m.ProcessedOn == null)
                    .OrderBy(m => m.OccurredOn)
                    .Take(50)
                    .ToListAsync(stoppingToken);

                if (messages.Any())
                {
                    using var producer = new ProducerBuilder<Null, string>(_producerConfig).Build();

                    foreach (var message in messages)
                    {
                        var kafkaMessage = new Message<Null, string> { Value = message.Payload };
                        
                        var result = await producer.ProduceAsync(TopicName, kafkaMessage, stoppingToken);

                        if (result.Status == PersistenceStatus.Persisted)
                        {
                            message.MarkAsProcessed();
                        }
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception)
            {
                // Log error
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}