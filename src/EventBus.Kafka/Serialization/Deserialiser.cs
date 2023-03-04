namespace EventBus.Kafka.Serialization;

internal class Deserialiser<T> : IDeserializer<T>
{
    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        //if (isNull)
        //{
        //    throw new ArgumentNullException(nameof(data));
        //}
        return JsonSerializer.Deserialize<T>(
            data,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true, })!;
    }
}
