namespace PaymentService.Infrastructure.Persistence.Repositories;

using PaymentService.Domain.Entities;
using PaymentService.Application.Common.Interfaces;

public class PaymentRepository : IPaymentRepository
{
    private readonly PaymentDbContext _context;

    public PaymentRepository(PaymentDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PaymentTransaction transaction, CancellationToken cancellationToken)
    {
        await _context.PaymentTransactions.AddAsync(transaction, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}