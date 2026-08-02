namespace AuctionService.Domain.Entities;

using AuctionService.Domain.Common;
using AuctionService.Domain.Enums;

public class Auction : EntityBase
{
    public Guid ProductId { get; private set; }
    public Guid SellerId { get; private set; }
    public Guid? HighestBidderId { get; private set; }
    public decimal StartingPrice { get; private set; }
    public decimal CurrentPrice { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public AuctionStatus Status { get; private set; }

    private Auction() { }

    public Auction(Guid productId, Guid sellerId, decimal startingPrice, DateTime startTime, DateTime endTime)
    {
        ProductId = productId;
        SellerId = sellerId;
        StartingPrice = startingPrice;
        CurrentPrice = startingPrice;
        StartTime = startTime;
        EndTime = endTime;
        Status = AuctionStatus.Pending;
    }

    public void UpdatePrice(decimal newPrice, Guid bidderId)
    {
        if (newPrice > CurrentPrice)
        {
            CurrentPrice = newPrice;
            HighestBidderId = bidderId;
        }
    }

    public void ChangeStatus(AuctionStatus status)
    {
        Status = status;
    }
}