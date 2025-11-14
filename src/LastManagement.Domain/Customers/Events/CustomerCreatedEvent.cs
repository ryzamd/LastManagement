using LastManagement.Domain.Common;

namespace LastManagement.Domain.Customers.Events;

public sealed record CustomerCreatedEvent(string CustomerName) : DomainEvent;