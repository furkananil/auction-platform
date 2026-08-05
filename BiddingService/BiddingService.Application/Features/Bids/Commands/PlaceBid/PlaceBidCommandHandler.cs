namespace BiddingService.Application.Features.Bids.Commands.PlaceBid;

using MediatR;
using System.Text.Json;
using BiddingService.Domain.Entities;
using BiddingService.Domain.Events;
using BiddingService.Domain.Repositories;
using BiddingService.Application.Common.Interfaces;

public class PlaceBidCommandHandler : IRequestHandler<PlaceBidCommand, Guid>
{
    private readonly IBidRepository _bidRepository;
    private readonly IOutboxRepository _outboxRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICatalogGrpcClient _catalogGrpcClient;

    public PlaceBidCommandHandler(
        IBidRepository bidRepository, 
        IOutboxRepository outboxRepository, 
        IUnitOfWork unitOfWork,
        ICatalogGrpcClient catalogGrpcClient)
    {
        _bidRepository = bidRepository;
        _outboxRepository = outboxRepository;
        _unitOfWork = unitOfWork;
        _catalogGrpcClient = catalogGrpcClient;
    }

    public async Task<Guid> Handle(PlaceBidCommand request, CancellationToken cancellationToken)
    {
        var productExists = await _catalogGrpcClient.CheckProductExistsAsync(request.ProductId, cancellationToken);
        
        if (!productExists)
        {
            throw new KeyNotFoundException("Product not found in Catalog Service.");
        }

        var bid = new Bid(
            request.AuctionId, 
            request.BidderId, 
            request.Amount, 
            request.IdempotencyKey);

        var bidPlacedEvent = new BidPlacedEvent(
            bid.Id,
            bid.AuctionId,
            bid.BidderId,
            bid.Amount,
            bid.CreatedAt);

        var outboxMessage = new OutboxMessage(
            nameof(BidPlacedEvent),
            JsonSerializer.Serialize(bidPlacedEvent));

        await _bidRepository.AddAsync(bid, cancellationToken);
        await _outboxRepository.AddAsync(outboxMessage, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return bid.Id;
    }
}