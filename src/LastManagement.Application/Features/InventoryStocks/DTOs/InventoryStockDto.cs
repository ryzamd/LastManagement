public class InventoryStockDto
{
    public int Id { get; set; }
    public int LastId { get; set; }
    public int SizeId { get; set; }
    public int LocationId { get; set; }
    public int QuantityGood { get; set; }
    public int QuantityDamaged { get; set; }
    public int QuantityReserved { get; set; }
    public int AvailableQuantity { get; set; }
    public DateTime LastUpdated { get; set; }
    public int Version { get; set; }
}