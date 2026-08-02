namespace AuctionService.Infrastructure.BackgroundJobs;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using AuctionService.Domain.Entities;
using AuctionService.Domain.Enums;
using AuctionService.Domain.Repositories;
using AuctionService.Infrastructure.Persistence;
using AuctionService.Application.Features.Auctions.Events;

public class AuctionClosingBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public AuctionClosingBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();
                var cacheService = scope.ServiceProvider.GetRequiredService<IAuctionCacheService>();

                var expiredAuctions = await dbContext.Auctions
                    .Where(a => a.Status == AuctionStatus.Active && a.EndTime <= DateTime.UtcNow)
                    .ToListAsync(stoppingToken);

                if (expiredAuctions.Any())
                {
                    foreach (var auction in expiredAuctions)
                    {
                        auction.ChangeStatus(AuctionStatus.Closed);
                        
                        await cacheService.RemoveActiveAuctionAsync(auction.Id, stoppingToken);

                        var closedEvent = new AuctionClosedEvent(
                            auction.Id,
                            auction.ProductId,
                            auction.HighestBidderId,
                            auction.CurrentPrice);

                        var outboxMessage = new OutboxMessage(
                            nameof(AuctionClosedEvent),
                            JsonSerializer.Serialize(closedEvent));

                        await dbContext.OutboxMessages.AddAsync(outboxMessage, stoppingToken);
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception)
            {
                // Log error
            }

            await Task.Delay(10000, stoppingToken);
        }
    }
}