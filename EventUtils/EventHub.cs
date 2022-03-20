using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Reflection;

namespace EventUtils;

internal class EventHub : IEventHub
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentQueue<IDomainEvent> _events = new ConcurrentQueue<IDomainEvent>();

    public EventHub(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Publish <T>(T domainEvent)
        where T: IDomainEvent
    {
        _events.Enqueue(domainEvent);
    }

    public async Task HandleEventsAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested
            && !_events.IsEmpty)
        {
            if (_events.TryDequeue(out var domainEvent))
            {
                await HandleEventAsync(domainEvent, cancellationToken);
            }
            else
            {
                // avoid constantly trying to dequeue the same object
                return;
            }
        }
    }

    private Task HandleEventAsync(IDomainEvent domainEvent, CancellationToken cancellationToken) {
        var serviceType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
        var handlers = _serviceProvider.GetServices(serviceType);

        return Task.WhenAll(handlers.Select(x => HandleEventAsync_Impl(x!, domainEvent, cancellationToken)));
    }

    private Task HandleEventAsync_Impl(object x, IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var handlerType = x!.GetType();
        var handleMethod = handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync), BindingFlags.Public | BindingFlags.Instance)!;
        var generic = handleMethod.MakeGenericMethod(domainEvent.GetType());

        var task = (Task)generic.Invoke(x, new object[] { domainEvent, cancellationToken })!;
        return task;
    }
}