using EventUtils;
using Loads.Events;

namespace Loads.Handlers;

public abstract class CreateLoadEventHandler : IDomainEventHandler<CreateLoadEvent>
{
    public abstract Task HandleAsync(CreateLoadEvent domainEvent, CancellationToken cancellationToken);
}