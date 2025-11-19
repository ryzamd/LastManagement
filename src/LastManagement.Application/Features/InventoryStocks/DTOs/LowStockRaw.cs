namespace LastManagement.Application.Features.InventoryStocks.DTOs
{
    public class LowStockRaw
    {
        public int StockId { get; set; }
        public string LastCode { get; set; } = string.Empty;
        public string SizeLabel { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;
        public int AvailableQuantity { get; set; }
    }
}
