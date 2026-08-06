using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationDelivery.Worker.Consumers;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var config = hostContext.Configuration;

        services.AddLogging(logging => logging.AddConsole());

        services.AddMassTransit(x =>
        {
            x.AddConsumer<EmailDeliveryConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(config["RabbitMq:Host"] ?? "localhost", "/", h =>
                {
                    h.Username(config["RabbitMq:Username"] ?? "guest");
                    h.Password(config["RabbitMq:Password"] ?? "guest");
                });

                cfg.ReceiveEndpoint("email-delivery-queue", e =>
                {
                    e.ConfigureConsumer<EmailDeliveryConsumer>(context);
                });
            });
        });
    })
    .Build();

try
{
    await host.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine(ex);   // sorun devam ederse gerçek hatayı bize gösterecek
    throw;
}