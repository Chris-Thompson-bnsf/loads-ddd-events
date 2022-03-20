using EventUtils;
using Imports.Data;
using Imports.Events;

namespace Imports.Handlers;

internal class StoreLoadImportedEvent : IDomainEventHandler<LoadImportedEvent>
{
    private readonly ImportsDbContext _context;

    public StoreLoadImportedEvent(ImportsDbContext context)
    {
        _context = context;
    }

    public async Task HandleAsync(LoadImportedEvent domainEvent, CancellationToken cancellationToken)
    {
        var efModel = new SavedLoadImportedEvent
        {
            BolNumber = domainEvent.BolNumber,
            CustomerCode= domainEvent.Customer,
            DetailsJson = string.Empty,
        };

        await _context.LoadImportedEvents.AddAsync(efModel, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // TODO: Publish an event to create a load in the Loads BC
    }
}