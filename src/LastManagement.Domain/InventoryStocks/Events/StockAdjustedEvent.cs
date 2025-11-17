using LastManagement.Domain.Common;

namespace LastManagement.Domain.InventoryStocks;

public sealed record StockAdjustedEvent(
    int StockId,
    int LastId,
    int SizeId,
    int LocationId,
    AdjustmentType AdjustmentType,
    int Quantity,
    string Reason
) : DomainEvent;