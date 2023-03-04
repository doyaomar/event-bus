namespace EventBus.Kafka.Serialization;

internal class Serialiser<T> : ISerializer<T>
{
    public byte[] Serialize(T data, SerializationContext context) => JsonSerializer.SerializeToUtf8Bytes(
            data,
            data!.GetType(),
            new JsonSerializerOptions { WriteIndented = true, });
}
