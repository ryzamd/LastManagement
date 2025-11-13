using LastManagement.Domain.Common;

namespace LastManagement.Domain.Accounts.Events;

public sealed record AccountCreatedEvent(string Username, string FullName) : DomainEvent;