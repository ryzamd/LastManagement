using LastManagement.Application.Features.InventoryStocks.Interfaces;

namespace LastManagement.Application.Features.InventoryStocks.Queries;

public class GetInventoryStockByIdQuery
{
    private readonly IInventoryStockRepository _repository;

    public GetInventoryStockByIdQuery(IInventoryStockRepository repository)
    {
        _repository = repository;
    }

    public async Task<InventoryStockDto?> ExecuteAsync(int stockId, CancellationToken cancellationToken = default)
    {
        var stock = await _repository.GetByIdAsync(stockId, cancellationToken);
        if (stock == null) return null;

        return new InventoryStockDto
        {
            Id = stock.StockId,
            LastId = stock.LastId,
            SizeId = stock.SizeId,
            LocationId = stock.LocationId,
            QuantityGood = stock.QuantityGood,
            QuantityDamaged = stock.QuantityDamaged,
            QuantityReserved = stock.QuantityReserved,
            AvailableQuantity = stock.GetAvailableQuantity(),
            LastUpdated = stock.LastUpdated,
            Version = stock.Version
        };
    }
}