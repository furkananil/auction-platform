namespace BiddingService.Domain.Repositories;

using BiddingService.Domain.Entities;

public interface IOutboxRepository
{
    Task AddAsync(OutboxMessage message, CancellationToken cancellationToken);
}