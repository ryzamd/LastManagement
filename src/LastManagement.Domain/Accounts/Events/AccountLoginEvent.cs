using LastManagement.Domain.Common;

namespace LastManagement.Domain.Accounts.Events;

public sealed record AccountLoginEvent(string Username, DateTime LoginAt) : DomainEvent;