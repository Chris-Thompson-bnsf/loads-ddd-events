using EventUtils;

namespace TestApps.DDD.DomainEvents.WebAPI.HostedServices;

public class BackgroundEventHandler : IHostedService, IDisposable
{
    private readonly ILogger<BackgroundEventHandler> _logger;
    private readonly IEventHub _eventHub;

    private Timer? _timer = null;
    
    public BackgroundEventHandler(
        ILogger<BackgroundEventHandler> logger,
        IEventHub eventHub)
    {
        _logger = logger;
        _eventHub = eventHub;
    }

    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Background Event Handler is starting.");

        _timer = new Timer(HandleEventsAsync, null, TimeSpan.Zero, TimeSpan.FromSeconds(15));
        return Task.CompletedTask;
    }

    // must be async void, not async task
    // source: https://stackoverflow.com/a/42687604/5038805
    // state parameter is required even though not used
    private async void HandleEventsAsync(object? state)
    {
        await _eventHub.HandleEventsAsync(CancellationToken.None);
    }

    /// <summary>
    /// Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Background Event Handler is stopping.");

        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        _timer?.Dispose();
    }
}