namespace BiddingService.Infrastructure.BackgroundJobs;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Confluent.Kafka;
using BiddingService.Infrastructure.Persistence;

public class OutboxPublisherBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ProducerConfig _producerConfig;
    private const string TopicName = "bidding-events";

    public OutboxPublisherBackgroundService(IServiceProvider serviceProvider, IConfiguration configuration)
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
                var dbContext = scope.ServiceProvider.GetRequiredService<BiddingDbContext>();

                var messages = await dbContext.OutboxMessages
                    .Where(m => m.ProcessedOn == null)
                    .OrderBy(m => m.CreatedAt)
                    .Take(20)
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
            catch (Exception ex)
            {
                // Log error (omitted per rules)
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}