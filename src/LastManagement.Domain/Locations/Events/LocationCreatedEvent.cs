using LastManagement.Domain.Common;

namespace LastManagement.Domain.Locations.Events;

public sealed record LocationCreatedEvent(string LocationCode, string LocationName) : DomainEvent;