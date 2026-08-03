namespace PaymentService.Domain.Entities;

using PaymentService.Domain.Enums;

public class PaymentTransaction
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid BuyerId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private PaymentTransaction() { }

    public PaymentTransaction(Guid orderId, Guid buyerId, decimal amount)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        BuyerId = buyerId;
        Amount = amount;
        Status = PaymentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(PaymentStatus status)
    {
        Status = status;
    }
}