namespace PaymentService.Api.Services;

using Grpc.Core;
using PaymentService.Api.Protos;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Enums;
using PaymentService.Application.Common.Interfaces;

public class PaymentGrpcService : PaymentGrpc.PaymentGrpcBase
{
    private readonly IPaymentRepository _paymentRepository;

    public PaymentGrpcService(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public override async Task<PaymentResponse> ProcessPayment(PaymentRequest request, ServerCallContext context)
    {
        try
        {
            var orderId = Guid.Parse(request.OrderId);
            var buyerId = Guid.Parse(request.BuyerId);
            var amount = (decimal)request.Amount;

            var transaction = new PaymentTransaction(orderId, buyerId, amount);
            
            // Simüle edilen başarılı ödeme işlemi
            transaction.UpdateStatus(PaymentStatus.Completed);

            await _paymentRepository.AddAsync(transaction, context.CancellationToken);
            await _paymentRepository.SaveChangesAsync(context.CancellationToken);

            return new PaymentResponse
            {
                Success = true,
                TransactionId = transaction.Id.ToString(),
                Message = "Payment processed successfully."
            };
        }
        catch (Exception ex)
        {
            return new PaymentResponse
            {
                Success = false,
                Message = $"Payment failed: {ex.Message}"
            };
        }
    }
}