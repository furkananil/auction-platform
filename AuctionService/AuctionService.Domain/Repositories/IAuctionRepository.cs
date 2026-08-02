namespace AuctionService.Domain.Repositories;

using AuctionService.Domain.Entities;

public interface IAuctionRepository
{
    Task<Auction?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Auction auction, CancellationToken cancellationToken);
    Task UpdateAsync(Auction auction, CancellationToken cancellationToken);
}