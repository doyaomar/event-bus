namespace EventBus.Abstractions;

public interface IEventBus
{
    void Publish(IntegrationEvent @event);

    Task PublishAsync(IntegrationEvent @event);

    void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>;

    Task SubscribeAsync<T, TH>()
    where T : IntegrationEvent
    where TH : IIntegrationEventHandler<T>;
}
