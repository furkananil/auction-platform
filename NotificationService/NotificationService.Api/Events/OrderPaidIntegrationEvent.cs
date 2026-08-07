namespace OrderingService.Infrastructure.Events;

public record OrderPaidIntegrationEvent(Guid OrderId, Guid BuyerId, decimal TotalAmount);