namespace AuctionService.Application.Features.Auctions.Commands.CreateAuction;

using MediatR;
using System.Text.Json;
using AuctionService.Domain.Entities;
using AuctionService.Domain.Enums;
using AuctionService.Domain.Events;
using AuctionService.Domain.Repositories;
using AuctionService.Application.Common.Interfaces;

public class CreateAuctionCommandHandler : IRequestHandler<CreateAuctionCommand, Guid>
{
    private readonly IAuctionRepository _auctionRepository;
    private readonly IAuctionCacheService _auctionCacheService;
    private readonly IOutboxRepository _outboxRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAuctionCommandHandler(
        IAuctionRepository auctionRepository,
        IAuctionCacheService auctionCacheService,
        IOutboxRepository outboxRepository,
        IUnitOfWork unitOfWork)
    {
        _auctionRepository = auctionRepository;
        _auctionCacheService = auctionCacheService;
        _outboxRepository = outboxRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateAuctionCommand request, CancellationToken cancellationToken)
    {
        var auction = new Auction(
            request.ProductId,
            request.SellerId,
            request.StartingPrice,
            request.StartTime,
            request.EndTime);

        if (request.StartTime <= DateTime.UtcNow)
        {
            auction.ChangeStatus(AuctionStatus.Active);
        }

        await _auctionRepository.AddAsync(auction, cancellationToken);

        if (auction.Status == AuctionStatus.Active)
        {
            await _auctionCacheService.SetActiveAuctionAsync(auction, cancellationToken);
        }

        var auctionOpenedEvent = new AuctionOpenedEvent(
            auction.Id,
            auction.ProductId,
            auction.SellerId,
            auction.StartingPrice,
            auction.EndTime);

        var outboxMessage = new OutboxMessage(
            nameof(AuctionOpenedEvent),
            JsonSerializer.Serialize(auctionOpenedEvent));

        await _outboxRepository.AddAsync(outboxMessage, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return auction.Id;
    }
}