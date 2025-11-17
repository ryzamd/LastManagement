using LastManagement.Domain.Common;

namespace LastManagement.Domain.LastSizes.Events;

public record LastSizeDiscontinuedEvent(int SizeId, int? ReplacementSizeId) : DomainEvent;
