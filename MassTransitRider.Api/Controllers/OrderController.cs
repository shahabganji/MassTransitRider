using MassTransit;
using MassTransitRider.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace MassTransitRider.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> _logger;
    private readonly IPublishEndpoint _publisher;
    private readonly IEventHubProducerProvider _eventHubProducerProvider;

    public OrderController(ILogger<OrderController> logger,
        IPublishEndpoint publisher,
        IEventHubProducerProvider eventHubProducerProvider)
    {
        _logger = logger;
        _publisher = publisher;
        _eventHubProducerProvider = eventHubProducerProvider;
    }

    [HttpPost(Name = "CreateNewOrder")]
    public async Task<Guid> Get()
    {
        var orderId = Guid.NewGuid();
        var date = DateTimeOffset.UtcNow;

        await _publisher.Publish<OrderCreated>(new
        {
            Id = orderId,
            CreatedAt = date
        });
        
        var producer = await _eventHubProducerProvider.GetProducer(new Uri("topic://evh-consumer"));
        await producer.Produce<OrderCreated>(new
        {
            OrderId = orderId, CreatedAt = date
        });

        _logger.LogInformation("A new order at {Date} is created: {OrderId}", date, orderId);

        return orderId;
    }
}