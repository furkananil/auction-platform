namespace AuctionService.Domain.Events;

public record AuctionOpenedEvent(
    Guid AuctionId, 
    Guid ProductId, 
    Guid SellerId, 
    decimal StartingPrice, 
    DateTime EndTime);