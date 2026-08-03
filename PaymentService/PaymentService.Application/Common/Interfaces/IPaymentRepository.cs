namespace PaymentService.Application.Common.Interfaces;

using PaymentService.Domain.Entities;

public interface IPaymentRepository
{
    Task AddAsync(PaymentTransaction transaction, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}