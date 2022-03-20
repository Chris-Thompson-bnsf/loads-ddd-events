using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace EventUtils;

internal class EventHub : IEventHub
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentQueue<(IDomainEvent, Func<Task>)> _events = new();

    public EventHub(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Publish <T>(T domainEvent)
        where T: IDomainEvent
    {
        _events.Enqueue((domainEvent, () =>
                {
                    var handlers = _serviceProvider.GetServices<IDomainEventHandler<T>>();
                    return Task.WhenAll(handlers.Select(x => x.HandleAsync(domainEvent, CancellationToken.None)));
                }
            ));
    }

    public async Task HandleEventsAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested
            && !_events.IsEmpty)
        {
            if (_events.TryDequeue(out var domainEvent))
            {
                var (e, handleAsync) = domainEvent;

                await handleAsync();
            }
            else
            {
                // avoid constantly trying to dequeue the same object
                return;
            }
        }
    }
}