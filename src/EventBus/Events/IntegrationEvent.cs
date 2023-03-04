namespace EventBus.Events;

public abstract class IntegrationEvent
{
    public DateTime CreationDate { get; }

    public Guid Id { get; }

    protected IntegrationEvent()
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
    }

    protected IntegrationEvent(Guid id, DateTime creationDate)
    {
        Id = id;
        CreationDate = creationDate;
    }
}
