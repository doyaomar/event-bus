namespace EventBus;

public class EventBusSubscriptionManager : IEventBusSubscriptionManager
{
    private readonly List<Type> _eventTypes;
    private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;

    public EventBusSubscriptionManager()
    {
        _handlers = new Dictionary<string, List<SubscriptionInfo>>();
        _eventTypes = new List<Type>();
    }

    public static string GetEventName<T>() where T : IntegrationEvent => typeof(T).Name;

    public void AddSubscription<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        string eventName = GetEventName<T>();
        Type eventType = typeof(T);
        Type handlerType = typeof(TH);

        if (!HasSubscriptionForEvent(eventName))
        {
            _handlers.Add(eventName, new List<SubscriptionInfo>());
        }

        if (!_eventTypes.Contains(eventType))
        {
            _eventTypes.Add(eventType);
        }

        if (_handlers[eventName].Any(si => si.HandlerType == handlerType))
        {
            // TODO : create custom exception
            throw new ArgumentException($"Handler Type {handlerType.Name} already exists for '{eventName}'", handlerType.Name);
        }

        _handlers[eventName].Add(new SubscriptionInfo(handlerType));
    }

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent
    {
        return GetHandlersForEvent(eventName: GetEventName<T>());
    }

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName) => _handlers[eventName];

    public bool HasSubscriptionForEvent<T>() where T : IntegrationEvent
    {
        return HasSubscriptionForEvent(eventName: GetEventName<T>());
    }

    public bool HasSubscriptionForEvent(string eventName) => _handlers.ContainsKey(eventName);
}
