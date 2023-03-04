namespace EventBus.Kafka;

internal class EventBusKafka : IEventBus
{
    private readonly IKafkaConnection _kafkaConnection;
    private readonly IEventBusSubscriptionManager _subscriptionManager;

    public EventBusKafka(IEventBusSubscriptionManager subscriptionManager, IKafkaConnection kafkaConnection)
    {
        _subscriptionManager = subscriptionManager ?? throw new ArgumentNullException(nameof(subscriptionManager));
        _kafkaConnection = kafkaConnection ?? throw new ArgumentNullException(nameof(kafkaConnection));
    }

    public void Publish(IntegrationEvent @event)
    {
        string eventName = EventBusSubscriptionManager.GetEventName<IntegrationEvent>();
        using IProducer<Guid, IntegrationEvent> producer = GetProducer();
        producer.Produce(eventName, new Message<Guid, IntegrationEvent>() { Key = @event.Id, Value = @event });
    }

    public async Task PublishAsync(IntegrationEvent @event)
    {
        string eventName = EventBusSubscriptionManager.GetEventName<IntegrationEvent>();
        using IProducer<Guid, IntegrationEvent> producer = GetProducer();
        await producer
            .ProduceAsync(eventName, new Message<Guid, IntegrationEvent>() { Key = @event.Id, Value = @event })
            .ConfigureAwait(false);
    }

    public void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        throw new NotImplementedException();
    }

    public Task SubscribeAsync<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        throw new NotImplementedException();
    }

    private IProducer<Guid, IntegrationEvent> GetProducer() => _kafkaConnection.Producer<IntegrationEvent>() ?? throw new InvalidOperationException("Producer Config is not valid.");
}