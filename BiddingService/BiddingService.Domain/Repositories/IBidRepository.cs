namespace BiddingService.Domain.Repositories;

using BiddingService.Domain.Entities;

public interface IBidRepository
{
    Task AddAsync(Bid bid, CancellationToken cancellationToken);
}