namespace EventBus.Kafka;

internal class EventBusKafka : IEventBus
{
    private readonly IKafkaConnection _kafkaConnection;
    private readonly IServiceProvider _services;
    private readonly IEventBusSubscriptionManager _subscriptionManager;
    public EventBusKafka(IEventBusSubscriptionManager subscriptionManager, IKafkaConnection kafkaConnection, IServiceProvider services)
    {
        _subscriptionManager = subscriptionManager ?? throw new ArgumentNullException(nameof(subscriptionManager));
        _kafkaConnection = kafkaConnection ?? throw new ArgumentNullException(nameof(kafkaConnection));
        _services = services ?? throw new ArgumentNullException(nameof(services));
    }

    public void Publish(IntegrationEvent @event)
    {
        using IProducer<Guid, IntegrationEvent> producer = GetProducer();
        string eventName = EventBusSubscriptionManager.GetEventName(@event);
        producer.Produce(eventName, new Message<Guid, IntegrationEvent>() { Key = @event.Id, Value = @event, });
    }

    public async Task PublishAsync(IntegrationEvent @event)
    {
        using IProducer<Guid, IntegrationEvent> producer = GetProducer();
        string eventName = EventBusSubscriptionManager.GetEventName(@event);
        await producer
            .ProduceAsync(eventName, new Message<Guid, IntegrationEvent>() { Key = @event.Id, Value = @event, })
            .ConfigureAwait(false);
    }

    public void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        using (var consumer = GetConsumer<T>())
        {
            string eventName = EventBusSubscriptionManager.GetEventName<T>();
            _subscriptionManager.AddSubscription<T, TH>();
            consumer.Subscribe(eventName);
            var cancellationToken = new CancellationToken();

            while (!cancellationToken.IsCancellationRequested)
            {
                ConsumeResult<Guid, T> consumerResult = consumer.Consume(cancellationToken);

                if (_subscriptionManager.HasSubscriptionForEvent<T>())
                {
                    var subscriptions = _subscriptionManager.GetHandlersForEvent<T>();

                    foreach (var subscription in subscriptions)
                    {
                        var handler = _services.GetRequiredService(subscription.HandlerType);

                        if (handler is null)
                        {
                            continue;
                        }

                        var handlerType = handler.GetType();
                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(handlerType);
                        concreteType.GetMethod("Handle")!.Invoke(handler, new object[] { consumerResult.Message.Value });
                    }
                }
            }

            consumer.Close();
        }
    }

    public async Task SubscribeAsync<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        using (var consumer = GetConsumer<T>())
        {
            string eventName = EventBusSubscriptionManager.GetEventName<T>();
            _subscriptionManager.AddSubscription<T, TH>();
            consumer.Subscribe(eventName);
            var cancellationToken = new CancellationToken();

            await Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    ConsumeResult<Guid, T> consumerResult = consumer.Consume(cancellationToken);

                    if (_subscriptionManager.HasSubscriptionForEvent<T>())
                    {
                        var subscriptions = _subscriptionManager.GetHandlersForEvent<T>();

                        foreach (var subscription in subscriptions)
                        {
                            var handler = _services.GetRequiredService(subscription.HandlerType);

                            if (handler is null)
                            {
                                continue;
                            }

                            var handlerType = handler.GetType();
                            var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(handlerType);
                            await Task.Yield();
                            await (Task)concreteType.GetMethod("Handle")!.Invoke(handler, new object[] { consumerResult.Message.Value })!;
                        }
                    }
                }
            }).ConfigureAwait(false);

            consumer.Close();
        }
    }

    private IConsumer<Guid, T> GetConsumer<T>() where T : IntegrationEvent
    {
        return _kafkaConnection.Consumer<T>() ?? throw new InvalidOperationException("Consumer config is not valid.");
    }

    private IProducer<Guid, IntegrationEvent> GetProducer() => _kafkaConnection.Producer<IntegrationEvent>() ?? throw new InvalidOperationException("Producer Config is not valid.");
}