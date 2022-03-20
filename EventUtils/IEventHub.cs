namespace EventUtils;

public interface IEventHub
{
    void Publish <T>(T domainEvent) where T: IDomainEvent;

    Task HandleEventsAsync(CancellationToken cancellationToken);
}