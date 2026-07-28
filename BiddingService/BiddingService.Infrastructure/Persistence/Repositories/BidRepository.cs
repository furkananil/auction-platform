namespace BiddingService.Infrastructure.Persistence.Repositories;

using BiddingService.Domain.Entities;
using BiddingService.Domain.Repositories;

public class BidRepository : IBidRepository
{
    private readonly BiddingDbContext _context;

    public BidRepository(BiddingDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Bid bid, CancellationToken cancellationToken)
    {
        await _context.Bids.AddAsync(bid, cancellationToken);
    }
}