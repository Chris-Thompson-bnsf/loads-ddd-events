namespace EventUtils;

public interface IEventHub
{
    Task Publish <T>(T domainEvent) where T: IDomainEvent;
}