namespace BiddingService.Domain.Entities;

using BiddingService.Domain.Common;

public class OutboxMessage : EntityBase
{
    public string Type { get; private set; }
    public string Payload { get; private set; }
    public DateTime? ProcessedOn { get; private set; }

    private OutboxMessage() { }

    public OutboxMessage(string type, string payload)
    {
        Type = type;
        Payload = payload;
    }

    public void MarkAsProcessed()
    {
        ProcessedOn = DateTime.UtcNow;
    }
}