using Microsoft.Extensions.DependencyInjection;

namespace EventUtils;

internal class EventHub : IEventHub
{
    private readonly IServiceProvider _serviceProvider;
    
    public EventHub(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Publish <T>(T domainEvent) where T: IDomainEvent
    {
        var handlers = _serviceProvider.GetServices<IDomainEventHandler<T>>();
        await Task.WhenAll(handlers.Select(x => x.HandleAsync(domainEvent, CancellationToken.None)));
    }
}