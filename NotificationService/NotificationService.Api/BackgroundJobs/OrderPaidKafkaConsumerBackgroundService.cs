namespace NotificationService.Api.BackgroundJobs;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Confluent.Kafka;
using System.Text.Json;
using MassTransit;
using Shared.Contracts;
using OrderingService.Infrastructure.Events;

public class OrderPaidKafkaConsumerBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConsumerConfig _consumerConfig;
    private const string TopicName = "order-events";

    public OrderPaidKafkaConsumerBackgroundService(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            GroupId = "notification-service-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build();
        consumer.Subscribe(TopicName);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = consumer.Consume(stoppingToken);
                if (consumeResult == null) continue;

                var orderPaidEvent = JsonSerializer.Deserialize<OrderPaidIntegrationEvent>(consumeResult.Message.Value);
                
                if (orderPaidEvent != null)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                    await publishEndpoint.Publish(new SendEmailCommand(
                        $"{orderPaidEvent.BuyerId}@ornek.com",
                        "Siparişiniz Ödendi!",
                        $"Tutar: {orderPaidEvent.TotalAmount} TL, Sipariş No: {orderPaidEvent.OrderId}"), 
                        stoppingToken);
                }
            }
            catch (Exception)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        consumer.Close();
    }
}