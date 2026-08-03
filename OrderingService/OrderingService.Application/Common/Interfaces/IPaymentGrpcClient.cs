namespace OrderingService.Application.Common.Interfaces;

public interface IPaymentGrpcClient
{
    Task<bool> ProcessPaymentAsync(string orderId, string buyerId, double amount, CancellationToken cancellationToken);
}