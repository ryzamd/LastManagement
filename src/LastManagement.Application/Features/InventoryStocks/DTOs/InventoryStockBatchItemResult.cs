namespace LastManagement.Application.Features.InventoryStocks.DTOs
{
    public class InventoryStockBatchItemResult
    {
        public int StockId { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? MovementId { get; set; }
        public int? NewQuantity { get; set; }
        public InventoryStockBatchError? Error { get; set; }
    }
}
