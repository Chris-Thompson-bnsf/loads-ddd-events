using SharedKernel.EfCore;
using WebAPI.HostedServices;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddDbContextMigrationServices(
        this IServiceCollection services)
    {
        return services.AddSingleton<MigrateContextService>()
           .AddHostedService<MigrateDbContextsHostedService>();
    }
}