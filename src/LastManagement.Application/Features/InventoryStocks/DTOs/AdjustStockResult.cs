using LastManagement.Domain.InventoryStocks;

public class AdjustStockResult
{
    public int StockId { get; set; }
    public AdjustmentType AdjustmentType { get; set; }
    public int PreviousQuantityGood { get; set; }
    public int NewQuantityGood { get; set; }
    public int PreviousQuantityDamaged { get; set; }
    public int NewQuantityDamaged { get; set; }
    public int Version { get; set; }
    public int MovementId { get; set; }
}