namespace AuctionService.Infrastructure.Persistence.Repositories;

using AuctionService.Domain.Entities;
using AuctionService.Domain.Repositories;

public class OutboxRepository : IOutboxRepository
{
    private readonly AuctionDbContext _context;

    public OutboxRepository(AuctionDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken)
    {
        await _context.OutboxMessages.AddAsync(message, cancellationToken);
    }
}