using LastManagement.Domain.Common;

namespace LastManagement.Domain.LastSizes.Events;

public record LastSizeReactivatedEvent(int SizeId) : DomainEvent;
