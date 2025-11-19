namespace LastManagement.Application.Features.InventoryStocks.DTOs
{
    public class InventorySummaryRaw
    {
        public int LastId { get; set; }
        public string LastCode { get; set; } = string.Empty;
        public string LastStatus { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;
        public string SizeLabel { get; set; } = string.Empty;
        public int QuantityGood { get; set; }
        public int QuantityDamaged { get; set; }
        public int QuantityReserved { get; set; }
        public int AvailableQuantity { get; set; }
    }
}
