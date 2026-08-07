namespace OrderingService.Infrastructure.BackgroundJobs;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Confluent.Kafka;
using System.Text.Json;
using OrderingService.Domain.Entities;
using OrderingService.Domain.Enums;
using OrderingService.Application.Common.Interfaces;
using OrderingService.Infrastructure.Events;

public class AuctionClosedConsumerBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConsumerConfig _consumerConfig;
    private const string TopicName = "auction-events";

    public AuctionClosedConsumerBackgroundService(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            GroupId = "ordering-service-group",
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

                var closedEvent = JsonSerializer.Deserialize<AuctionClosedIntegrationEvent>(consumeResult.Message.Value);
                
                if (closedEvent != null && closedEvent.WinnerId.HasValue)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var repository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
                    var paymentClient = scope.ServiceProvider.GetRequiredService<IPaymentGrpcClient>();

                    var order = new Order(
                        closedEvent.AuctionId,
                        closedEvent.ProductId,
                        closedEvent.WinnerId.Value,
                        closedEvent.FinalPrice);

                    await repository.AddAsync(order, stoppingToken);
                    await repository.SaveChangesAsync(stoppingToken);

                    var paymentSuccess = await paymentClient.ProcessPaymentAsync(
                        order.Id.ToString(), 
                        order.BuyerId.ToString(), 
                        (double)order.TotalAmount, 
                        stoppingToken);

                    if (paymentSuccess)
                    {
                        order.UpdateStatus(OrderStatus.Paid);
                        await repository.SaveChangesAsync(stoppingToken);

                        var producerConfig = new ProducerConfig { BootstrapServers = _consumerConfig.BootstrapServers };
                        using var producer = new ProducerBuilder<Null, string>(producerConfig).Build();
                        
                        var orderPaidEvent = new OrderPaidIntegrationEvent(order.Id, order.BuyerId, order.TotalAmount);
                        
                        await producer.ProduceAsync("order-events", new Message<Null, string> 
                        { 
                            Value = JsonSerializer.Serialize(orderPaidEvent) 
                        }, stoppingToken);
                    }
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