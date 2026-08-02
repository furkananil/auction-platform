namespace AuctionService.Infrastructure.Caching;

using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using AuctionService.Domain.Entities;
using AuctionService.Domain.Repositories;

public class AuctionCacheService : IAuctionCacheService
{
    private readonly IDistributedCache _cache;
    private const string KeyPrefix = "auction:";

    public AuctionCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task SetActiveAuctionAsync(Auction auction, CancellationToken cancellationToken)
    {
        var key = $"{KeyPrefix}{auction.Id}";
        var jsonData = JsonSerializer.Serialize(auction);
        await _cache.SetStringAsync(key, jsonData, cancellationToken);
    }

    public async Task<Auction?> GetActiveAuctionAsync(Guid id, CancellationToken cancellationToken)
    {
        var key = $"{KeyPrefix}{id}";
        var jsonData = await _cache.GetStringAsync(key, cancellationToken);

        if (string.IsNullOrEmpty(jsonData))
            return null;

        return JsonSerializer.Deserialize<Auction>(jsonData);
    }

    public async Task RemoveActiveAuctionAsync(Guid id, CancellationToken cancellationToken)
    {
        var key = $"{KeyPrefix}{id}";
        await _cache.RemoveAsync(key, cancellationToken);
    }
}