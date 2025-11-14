using LastManagement.Domain.Common;
using LastManagement.Domain.Customers.Events;

namespace LastManagement.Domain.Customers;

public sealed class Customer : Entity
{
    private Customer() { } // EF Core

    private Customer(string customerName, CustomerStatus status)
    {
        CustomerName = customerName;
        Status = status;
        AddDomainEvent(new CustomerCreatedEvent(customerName));
    }

    public string CustomerName { get; private set; } = string.Empty;
    public CustomerStatus Status { get; private set; }

    public static Customer Create(string customerName, CustomerStatus status = CustomerStatus.Active)
    {
        if (string.IsNullOrWhiteSpace(customerName))
            throw new ArgumentException("Customer name cannot be empty", nameof(customerName));

        if (customerName.Length > 200)
            throw new ArgumentException("Customer name cannot exceed 200 characters", nameof(customerName));

        return new Customer(customerName, status);
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Customer name cannot be empty", nameof(newName));

        if (newName.Length > 200)
            throw new ArgumentException("Customer name cannot exceed 200 characters", nameof(newName));

        CustomerName = newName;
        IncrementVersion();
    }

    public void UpdateStatus(CustomerStatus newStatus)
    {
        if (Status == newStatus) return;

        Status = newStatus;
        IncrementVersion();
        AddDomainEvent(new CustomerStatusChangedEvent(Id, Status));
    }
}