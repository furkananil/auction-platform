namespace BiddingService.Domain.Events;

public record BidPlacedEvent(
    Guid BidId,
    Guid AuctionId,
    Guid BidderId,
    decimal Amount,
    DateTime Timestamp);