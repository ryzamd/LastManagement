using LastManagement.Domain.Common;

namespace LastManagement.Domain.LastSizes.Events;

public sealed record LastSizeCreatedEvent(int SizeId, decimal SizeValue, string SizeLabel) : DomainEvent;