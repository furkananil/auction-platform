namespace OrderingService.Infrastructure.Clients;

using OrderingService.Application.Common.Interfaces;
using PaymentService.Api.Protos;

public class PaymentGrpcClient : IPaymentGrpcClient
{
    private readonly PaymentGrpc.PaymentGrpcClient _client;

    public PaymentGrpcClient(PaymentGrpc.PaymentGrpcClient client)
    {
        _client = client;
    }

    public async Task<bool> ProcessPaymentAsync(string orderId, string buyerId, double amount, CancellationToken cancellationToken)
    {
        var request = new PaymentRequest 
        { 
            OrderId = orderId, 
            BuyerId = buyerId, 
            Amount = amount 
        };

        var response = await _client.ProcessPaymentAsync(request, cancellationToken: cancellationToken);
        return response.Success;
    }
}