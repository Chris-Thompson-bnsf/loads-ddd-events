using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace EventUtils;

internal class EventHub : IEventHub
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventHub> _logger;

    private class EventToProcess
    {
        internal IDomainEvent DomainEvent { get; init; }
        internal Func<Task> HandleAsync { get; init; }
        internal long HandleAttempts { get; init; }
        
        public EventToProcess(IDomainEvent domainEvent, Func<Task> handleAsync)
            : this(domainEvent, handleAsync, 0)
        {
        }
        
        public EventToProcess(IDomainEvent domainEvent, Func<Task> handleAsync, long handleAttempts)
        {
            DomainEvent = domainEvent;
            HandleAsync = handleAsync;
            HandleAttempts = handleAttempts;
        }

        public void Deconstruct(out IDomainEvent domainEvent, out Func<Task> handleAsync, out long attempts)
        {
            domainEvent = DomainEvent;
            handleAsync = HandleAsync;
            attempts = HandleAttempts;
        }
    }

    private readonly ConcurrentQueue<EventToProcess> _events = new();

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

        _events.Enqueue(new EventToProcess(domainEvent, () =>
        {
            var handlers = _serviceProvider.GetServices<IDomainEventHandler<T>>();
            return Task.WhenAll(handlers.Select(x => x.HandleAsync(domainEvent, CancellationToken.None)));
        }));
    }

    public async Task HandleEventsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing events - {Count} remaining.", _events.Count);

        while (_events.TryDequeue(out var item))
        {
            var (e, handleAsync, attempts) = item;
            _logger.LogInformation("Processing {EventType} event for the {Attempts} time.", e.GetType().FullName, attempts);

            // we cannot guarantee that an implementation of the handler did not, for some reason, throw
            // but if so, we shouldn't crash the handling pipeline
            try
            {
                await handleAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process event - stopping current processing.");
                // the event was dequeued, but failed to be processed, so put it back into the queue for later re-processing
                _events.Enqueue(new EventToProcess(e, handleAsync, attempts + 1));
                return;
            }
        }
    }
}