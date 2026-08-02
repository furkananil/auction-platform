namespace AuctionService.Domain.Repositories;

using AuctionService.Domain.Entities;

public interface IAuctionCacheService
{
    Task SetActiveAuctionAsync(Auction auction, CancellationToken cancellationToken);
    Task<Auction?> GetActiveAuctionAsync(Guid id, CancellationToken cancellationToken);
    Task RemoveActiveAuctionAsync(Guid id, CancellationToken cancellationToken);
}