namespace AuctionService.Infrastructure.BackgroundJobs;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Confluent.Kafka;
using System.Text.Json;
using AuctionService.Infrastructure.Persistence;
using AuctionService.Domain.Repositories;

public class AuctionBidConsumerBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConsumerConfig _consumerConfig;
    private const string TopicName = "bidding-events";

    public AuctionBidConsumerBackgroundService(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            GroupId = "auction-service-group",
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
                if (bidEvent == null) continue;

                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();
                var cacheService = scope.ServiceProvider.GetRequiredService<IAuctionCacheService>();

                var auction = await dbContext.Auctions.FindAsync(new object[] { bidEvent.AuctionId }, stoppingToken);

                if (auction != null && bidEvent.Amount > auction.CurrentPrice)
                {
                    auction.UpdatePrice(bidEvent.Amount, bidEvent.BidderId);
                    
                    await dbContext.SaveChangesAsync(stoppingToken);
                    await cacheService.SetActiveAuctionAsync(auction, stoppingToken);
                }
            }
            catch (Exception)
            {
                // Log error and continue
                await Task.Delay(1000, stoppingToken);
            }
        }

        consumer.Close();
    }
}