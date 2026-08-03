namespace OrderingService.Domain.Entities;

using OrderingService.Domain.Enums;

public class Order
{
    public Guid Id { get; private set; }
    public Guid AuctionId { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid BuyerId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Order() { }

    public Order(Guid auctionId, Guid productId, Guid buyerId, decimal totalAmount)
    {
        Id = Guid.NewGuid();
        AuctionId = auctionId;
        ProductId = productId;
        BuyerId = buyerId;
        TotalAmount = totalAmount;
        Status = OrderStatus.PendingPayment;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(OrderStatus status)
    {
        Status = status;
    }
}