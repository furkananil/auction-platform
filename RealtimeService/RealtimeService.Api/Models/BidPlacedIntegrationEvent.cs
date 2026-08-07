namespace RealtimeService.Api.Models;

public record BidPlacedIntegrationEvent(
    Guid BidId,
    Guid AuctionId,
    Guid BidderId,
    decimal Amount,
    DateTime Timestamp);