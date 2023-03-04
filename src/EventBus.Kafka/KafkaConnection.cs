namespace EventBus.Kafka;

internal class KafkaConnection : IKafkaConnection
{
    private readonly KafkaConfiguration _kafkaConfig;

    public KafkaConnection(IOptions<KafkaConfiguration> kafkaConfig)
    {
        _kafkaConfig = kafkaConfig?.Value ?? throw new ArgumentNullException(nameof(kafkaConfig));
    }

    public IConsumer<Guid, T>? Consumer<T>() where T : IntegrationEvent
    {
        if (_kafkaConfig.ConsumerConfig is null)
        {
            return null;
        }

        return new ConsumerBuilder<Guid, T>(_kafkaConfig.ConsumerConfig)
            .SetKeyDeserializer(new Deserialiser<Guid>())
            .SetValueDeserializer(new Deserialiser<T>())
            .Build();
    }

    public IProducer<Guid, T>? Producer<T>() where T : IntegrationEvent
    {
        if (_kafkaConfig.ProducerConfig is null)
        {
            return null;
        }

        return new ProducerBuilder<Guid, T>(_kafkaConfig.ProducerConfig)
            .SetKeySerializer(new Serialiser<Guid>())
            .SetValueSerializer(new Serialiser<T>())
            .Build();
    }
}
