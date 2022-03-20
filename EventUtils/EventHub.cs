using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace EventUtils;

internal class EventHub : IEventHub
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventHub> _logger;

    private readonly ConcurrentQueue<(IDomainEvent, Func<Task>)> _events = new();

    public EventHub(
        IServiceProvider serviceProvider,
        ILogger<EventHub> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public void Publish<T>(T domainEvent)
        where T : IDomainEvent
    {
        _logger.LogInformation("Queueing new {EventType} event.", domainEvent.GetType().FullName);

        _events.Enqueue((domainEvent, () =>
                {
                    var handlers = _serviceProvider.GetServices<IDomainEventHandler<T>>();
                    return Task.WhenAll(handlers.Select(x => x.HandleAsync(domainEvent, CancellationToken.None)));
                }
        ));
    }

    public async Task HandleEventsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing events - {Count} remaining.", _events.Count);

        while (_events.TryDequeue(out var domainEvent))
        {
            var (e, handleAsync) = domainEvent;
            _logger.LogInformation("Processing {EventType} event.", e.GetType().FullName);

            // we cannot guarantee that an implementation of the handler did not, for some reason, throw
            // but if so, we shouldn't crash the handling pipeline
            try
            {
                await handleAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process event - stopping current processing.");
                return;
            }
        }
    }
}