namespace EventBus.Abstractions;

public interface IEventBusSubscriptionManager
{
    void AddSubscription<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>;

    IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);

    IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent;

    bool HasSubscriptionForEvent(string eventName);

    bool HasSubscriptionForEvent<T>() where T : IntegrationEvent;
}