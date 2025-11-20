using LastManagement.Domain.Common;
using LastManagement.Domain.PurchaseOrders.Enums;

namespace LastManagement.Domain.PurchaseOrders.Entities;

public sealed class PurchaseOrder : Entity
{
    public int OrderId { get; private set; }
    public string OrderNumber { get; private set; } = string.Empty;
    public PurchaseOrderStatus Status { get; private set; }
    public int LocationId { get; private set; }
    public string RequestedBy { get; private set; } = string.Empty;
    public string? Department { get; private set; }
    public string? Notes { get; private set; }
    public string? AdminNotes { get; private set; }
    public new DateTime CreatedAt { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public string? ReviewedBy { get; private set; }
    public new int Version { get; private set; }

    // Navigation property
    public ICollection<PurchaseOrderItem> Items { get; private set; } = new List<PurchaseOrderItem>();

    private PurchaseOrder() { } // EF Core

    public static PurchaseOrder Create(string orderNumber, int locationId, string requestedBy, string? department = null, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(orderNumber))
            throw new ArgumentException("Order number cannot be empty", nameof(orderNumber));

        if (string.IsNullOrWhiteSpace(requestedBy))
            throw new ArgumentException("Requested by cannot be empty", nameof(requestedBy));

        if (locationId <= 0)
            throw new ArgumentException("Location ID must be positive", nameof(locationId));

        return new PurchaseOrder
        {
            OrderNumber = orderNumber,
            LocationId = locationId,
            RequestedBy = requestedBy,
            Department = department,
            Notes = notes,
            Status = PurchaseOrderStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            Version = 1
        };
    }

    public void Confirm(string reviewedBy, string? adminNotes = null)
    {
        if (Status != PurchaseOrderStatus.Pending)
            throw new InvalidOperationException($"Cannot confirm order with status {Status}. Order must be Pending.");

        if (string.IsNullOrWhiteSpace(reviewedBy))
            throw new ArgumentException("Reviewed by cannot be empty", nameof(reviewedBy));

        Status = PurchaseOrderStatus.Confirmed;
        ReviewedAt = DateTime.UtcNow;
        ReviewedBy = reviewedBy;
        AdminNotes = adminNotes;
        Version++;
    }

    public void Deny(string reviewedBy, string? adminNotes = null)
    {
        if (Status != PurchaseOrderStatus.Pending)
            throw new InvalidOperationException($"Cannot deny order with status {Status}. Order must be Pending.");

        if (string.IsNullOrWhiteSpace(reviewedBy))
            throw new ArgumentException("Reviewed by cannot be empty", nameof(reviewedBy));

        Status = PurchaseOrderStatus.Denied;
        ReviewedAt = DateTime.UtcNow;
        ReviewedBy = reviewedBy;
        AdminNotes = adminNotes;
        Version++;
    }

    public void AddItem(PurchaseOrderItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        Items.Add(item);
    }

    public override void IncrementVersion()
    {
        Version++;
    }
}