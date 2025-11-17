public class TransferStockResult
{
    public string TransferId { get; set; } = string.Empty;
    public StockSnapshot FromStock { get; set; } = null!;
    public StockSnapshot ToStock { get; set; } = null!;
    public int MovementId { get; set; }
}