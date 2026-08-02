namespace AuctionService.Infrastructure.BackgroundJobs;

public record BidPlacedIntegrationEvent(Guid AuctionId, Guid BidderId, decimal Amount);