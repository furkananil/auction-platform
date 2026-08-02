namespace AuctionService.Domain.Repositories;

using AuctionService.Domain.Entities;

public interface IOutboxRepository
{
    Task AddAsync(OutboxMessage message, CancellationToken cancellationToken);
}