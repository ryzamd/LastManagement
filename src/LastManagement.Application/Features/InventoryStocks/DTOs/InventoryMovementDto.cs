namespace LastManagement.Application.Features.InventoryStocks.DTOs;

public class InventoryMovementDto
{
    public int Id { get; set; }
    public int LastId { get; set; }
    public string LastCode { get; set; } = string.Empty;
    public int SizeId { get; set; }
    public string SizeLabel { get; set; } = string.Empty;
    public int? FromLocationId { get; set; }
    public string? FromLocationName { get; set; }
    public int? ToLocationId { get; set; }
    public string? ToLocationName { get; set; }
    public string MovementType { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string? Reason { get; set; }
    public string? ReferenceNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public Dictionary<string, object> Links { get; set; } = new();
}