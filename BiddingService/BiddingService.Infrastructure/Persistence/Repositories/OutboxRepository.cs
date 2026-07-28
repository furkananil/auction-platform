namespace BiddingService.Infrastructure.Persistence.Repositories;

using BiddingService.Domain.Entities;
using BiddingService.Domain.Repositories;

public class OutboxRepository : IOutboxRepository
{
    private readonly BiddingDbContext _context;

    public OutboxRepository(BiddingDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken)
    {
        await _context.OutboxMessages.AddAsync(message, cancellationToken);
    }
}