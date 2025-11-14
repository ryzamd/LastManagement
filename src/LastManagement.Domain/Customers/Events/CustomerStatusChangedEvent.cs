using LastManagement.Domain.Common;

namespace LastManagement.Domain.Customers.Events;

public sealed record CustomerStatusChangedEvent(int CustomerId, CustomerStatus NewStatus) : DomainEvent;