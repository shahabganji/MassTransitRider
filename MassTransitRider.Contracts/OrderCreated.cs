    namespace MassTransitRider.Contracts;

public interface OrderCreated
{
    Guid Id { get; }
    DateTimeOffset CreatedAt { get; }
}
