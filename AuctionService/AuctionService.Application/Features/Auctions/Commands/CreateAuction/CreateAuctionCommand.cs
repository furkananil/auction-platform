namespace AuctionService.Application.Features.Auctions.Commands.CreateAuction;

using MediatR;

public record CreateAuctionCommand(
    Guid ProductId,
    Guid SellerId,
    decimal StartingPrice,
    DateTime StartTime,
    DateTime EndTime) : IRequest<Guid>;