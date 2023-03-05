namespace EventBus.Kafka.Extensions;

public static class ServiceExtensions
{
    public static IEventBus AddEventBusKafka(this IServiceCollection services)
    {
        services
            .AddSingleton<IKafkaConnection, KafkaConnection>()
            .AddSingleton<IEventBusSubscriptionManager, EventBusSubscriptionManager>()
            .AddSingleton<IEventBus, EventBusKafka>();
        var serviceProvider = services.BuildServiceProvider();

        return serviceProvider.GetRequiredService<IEventBus>();
    }
}
