namespace LastManagement.Application.Features.PurchaseOrders.DTOs;

public sealed class InventoryUpdateDto
{
    public int StockId { get; set; }
    public string LastCode { get; set; } = string.Empty;
    public string SizeLabel { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public int PreviousQuantity { get; set; }
    public int NewQuantity { get; set; }
    public int MovementId { get; set; }
}