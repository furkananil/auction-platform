namespace OrderingService.Infrastructure.Events;

public record AuctionClosedIntegrationEvent(
    Guid AuctionId, 
    Guid ProductId, 
    Guid? WinnerId, 
    decimal FinalPrice);