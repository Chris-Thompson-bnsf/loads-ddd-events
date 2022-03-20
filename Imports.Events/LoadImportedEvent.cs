using EventUtils;

namespace Imports.Events;

public class LoadImportedEvent : IDomainEvent
{
    public string BolNumber { get; init; }
    public string Customer { get; init; }
    public string DetailsJson { get; init; }
}