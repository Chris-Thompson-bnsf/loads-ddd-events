using Imports.Data;
using SharedKernel.EfCore;

namespace TestApps.DDD.DomainEvents.WebAPI.HostedServices;

public class MigrateDbContextsHostedService : IHostedService
{
    private readonly MigrateContextService _migrationService;
    
    public MigrateDbContextsHostedService(MigrateContextService migrationService)
    {
        _migrationService = migrationService;
    }

    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _migrationService.MigrateAsync<ImportsDbContext>(cancellationToken);
    }

    /// <summary>
    /// Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}