namespace BiddingService.Application.Features.Bids.Commands.PlaceBid;

using MediatR;

public record PlaceBidCommand(
    Guid AuctionId, 
    Guid ProductId,
    Guid BidderId, 
    decimal Amount, 
    string IdempotencyKey) : IRequest<Guid>;