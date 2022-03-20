using SharedKernel.EfCore;
using TestApps.DDD.DomainEvents.WebAPI.HostedServices;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddDbContextMigrationServices(
        this IServiceCollection services)
    {
        return services.AddSingleton<MigrateContextService>()
           .AddHostedService<MigrateDbContextsHostedService>()
           .AddHostedService<BackgroundEventHandler>()
            ;
    }
}