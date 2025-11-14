using LastManagement.Domain.Common;

namespace LastManagement.Domain.Locations.Events;

public sealed record LocationDeactivatedEvent(int LocationId, string LocationCode) : DomainEvent;