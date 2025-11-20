namespace LastManagement.Application.Features.PurchaseOrders.DTOs;

public sealed class PurchaseOrderItemDto
{
    public int Id { get; set; }
    public int LastId { get; set; }
    public string LastCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public int SizeId { get; set; }
    public string SizeLabel { get; set; } = string.Empty;
    public int QuantityRequested { get; set; }
}