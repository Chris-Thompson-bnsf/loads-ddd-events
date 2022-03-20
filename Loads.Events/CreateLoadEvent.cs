using EventUtils;

namespace Loads.Events;

public class CreateLoadEvent : IDomainEvent
{
    public string Customer { get; init; }
    public string BolNumber { get; init; }
}