namespace AuctionService.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using AuctionService.Domain.Entities;
using AuctionService.Domain.Repositories;

public class AuctionRepository : IAuctionRepository
{
    private readonly AuctionDbContext _context;

    public AuctionRepository(AuctionDbContext context)
    {
        _context = context;
    }

    public async Task<Auction?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Auctions.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task AddAsync(Auction auction, CancellationToken cancellationToken)
    {
        await _context.Auctions.AddAsync(auction, cancellationToken);
    }

    public async Task UpdateAsync(Auction auction, CancellationToken cancellationToken)
    {
        _context.Auctions.Update(auction);
        await Task.CompletedTask;
    }
}