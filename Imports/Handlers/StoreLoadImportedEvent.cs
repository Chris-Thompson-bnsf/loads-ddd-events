using EventUtils;
using Imports.Data;
using Imports.Events;
using Loads.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Imports.Handlers;

internal class StoreLoadImportedEvent : IDomainEventHandler<LoadImportedEvent>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IEventHub _eventHub;

    public StoreLoadImportedEvent(
        IServiceScopeFactory scopeFactory,
        IEventHub eventHub)
    {
        _scopeFactory = scopeFactory;
        _eventHub = eventHub;
    }

    public async Task HandleAsync(LoadImportedEvent domainEvent, CancellationToken cancellationToken)
    {
        var efModel = new SavedLoadImportedEvent
        {
            BolNumber = domainEvent.BolNumber,
            CustomerCode= domainEvent.Customer,
            DetailsJson = string.Empty,
        };

        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ImportsDbContext>();

        await context.LoadImportedEvents.AddAsync(efModel, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        await _eventHub.Publish(new CreateLoadEvent()
        {
            BolNumber = domainEvent.BolNumber,
            Customer = domainEvent.Customer,
        });
    }
}