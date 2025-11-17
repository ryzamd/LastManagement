using LastManagement.Domain.Common;

namespace LastManagement.Domain.InventoryStocks;

public sealed record StockTransferredEvent(
    int LastId,
    int SizeId,
    int FromLocationId,
    int ToLocationId,
    int Quantity,
    string Reason
) : DomainEvent;