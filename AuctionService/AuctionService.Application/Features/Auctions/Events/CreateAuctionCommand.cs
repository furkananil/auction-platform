namespace AuctionService.Application.Features.Auctions.Events;

public record AuctionClosedEvent(
    Guid AuctionId, 
    Guid ProductId, 
    Guid? WinnerId, 
    decimal FinalPrice);