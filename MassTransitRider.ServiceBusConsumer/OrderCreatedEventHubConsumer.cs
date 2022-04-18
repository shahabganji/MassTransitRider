using MassTransit;
using MassTransitRider.Contracts;

namespace MassTransitRider.ServiceBusConsumer;

public class OrderCreatedEventHubConsumer : IConsumer<OrderCreated>
{
    private readonly ILogger<OrderCreatedEventHubConsumer> _logger;

    public OrderCreatedEventHubConsumer(ILogger<OrderCreatedEventHubConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<OrderCreated> context)
    {
        _logger.LogInformation("EVH - Order {OrderId}, Created at {Date}, is consumed at {ConsumeDate}",
            context.Message.Id, context.Message.CreatedAt, DateTimeOffset.UtcNow);
        
        return Task.CompletedTask;
    }
}