public class StockSnapshot
{
    public int Id { get; set; }
    public int LocationId { get; set; }
    public int PreviousQuantity { get; set; }
    public int NewQuantity { get; set; }
    public int Version { get; set; }
}