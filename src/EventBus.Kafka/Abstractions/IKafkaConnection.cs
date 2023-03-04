namespace EventBus.Kafka.Abstractions;

internal interface IKafkaConnection
{
    IConsumer<Guid, T>? Consumer<T>() where T : IntegrationEvent;

    IProducer<Guid, T>? Producer<T>() where T : IntegrationEvent;
}