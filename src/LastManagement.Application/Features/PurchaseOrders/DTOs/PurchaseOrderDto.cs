namespace LastManagement.Application.Features.PurchaseOrders.DTOs;

public sealed class PurchaseOrderDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int LocationId { get; set; }
    public string? LocationName { get; set; }
    public string Status { get; set; } = string.Empty;
    public string RequestedBy { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Notes { get; set; }
    public string? AdminNotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewedBy { get; set; }
    public int ItemCount { get; set; }
    public int TotalQuantity { get; set; }
    public List<PurchaseOrderItemDto>? Items { get; set; }
}