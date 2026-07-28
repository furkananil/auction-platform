namespace RealtimeService.Api.Models;

public record BidPlacedEvent(
    Guid BidId,
    Guid AuctionId,
    Guid BidderId,
    decimal Amount,
    DateTime Timestamp);