namespace LastManagement.Application.Features.InventoryStocks.DTOs
{
    public class InventoryStockBatchOperationResult
    {
        public int Successful { get; set; }
        public int Failed { get; set; }
        public List<InventoryStockBatchItemResult> Results { get; set; } = new();
    }
}
