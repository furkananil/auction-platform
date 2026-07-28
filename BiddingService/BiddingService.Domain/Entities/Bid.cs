namespace BiddingService.Domain.Entities;

using BiddingService.Domain.Common;

public class Bid : EntityBase
{
    public Guid AuctionId { get; private set; }
    public Guid BidderId { get; private set; }
    public decimal Amount { get; private set; }
    public string IdempotencyKey { get; private set; }

    private Bid() { }

    public Bid(Guid auctionId, Guid bidderId, decimal amount, string idempotencyKey)
    {
        AuctionId = auctionId;
        BidderId = bidderId;
        Amount = amount;
        IdempotencyKey = idempotencyKey;
    }
}