namespace LastManagement.Application.Features.InventoryStocks.DTOs;

public class LowStockAlertDto
{
    public int StockId { get; set; }
    public string LastCode { get; set; } = string.Empty;
    public string SizeLabel { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public int AvailableQuantity { get; set; }
    public int Threshold { get; set; }
    public int RecommendedRestock { get; set; }
    public Dictionary<string, object> Links { get; set; } = new();
}

public class LowStockSummary
{
    public int TotalLowStockItems { get; set; }
    public int CriticalItems { get; set; }
    public int WarningItems { get; set; }
}