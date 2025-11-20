namespace LastManagement.Application.Features.PurchaseOrders.DTOs;

public sealed class PurchaseOrderSummaryDto
{
    public int TotalPending { get; set; }
    public int TotalConfirmed { get; set; }
    public int TotalDenied { get; set; }
    public int TotalItemsRequested { get; set; }
    public int TotalQuantityRequested { get; set; }
}