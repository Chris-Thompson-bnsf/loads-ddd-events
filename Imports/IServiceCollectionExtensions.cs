using Imports.Data;
using Imports.Events;
using Imports.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddLoadImportServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddDbContext<ImportsDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("LoadImportsDb")))
           .AddDomainEvent<LoadImportedEvent, StoreLoadImportedEvent>();
    }
}