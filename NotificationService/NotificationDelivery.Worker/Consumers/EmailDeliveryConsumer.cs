namespace NotificationDelivery.Worker.Consumers;

using MassTransit;
using Shared.Contracts;

public class EmailDeliveryConsumer : IConsumer<SendEmailCommand>
{
    public Task Consume(ConsumeContext<SendEmailCommand> context)
    {
        Console.WriteLine($"[EMAIL GÖNDERİLDİ] Alıcı: {context.Message.To} | Konu: {context.Message.Subject}");
        return Task.CompletedTask;
    }
}