using EventUtils;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddEventHub(this IServiceCollection services)
    {
        return services.AddSingleton<IEventHub, EventHub>();
    }

    public static IServiceCollection AddDomainEvent<TDomainEvent, THandler>(this IServiceCollection services)
        where TDomainEvent : class, IDomainEvent
        where THandler : class, IDomainEventHandler<TDomainEvent>
    {
        return services.AddSingleton<IDomainEventHandler<TDomainEvent>, THandler>();
    }
}