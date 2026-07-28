namespace BiddingService.Infrastructure.Persistence;

using BiddingService.Application.Common.Interfaces;

public class UnitOfWork : IUnitOfWork
{
    private readonly BiddingDbContext _context;

    public UnitOfWork(BiddingDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}