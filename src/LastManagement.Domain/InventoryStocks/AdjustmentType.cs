namespace LastManagement.Domain.InventoryStocks;

public enum AdjustmentType
{
    Add,      // Increase quantity_good (restock)
    Remove,   // Decrease quantity_good (usage/loss)
    Damage,   // Move from good to damaged
    Repair    // Move from damaged to good
}