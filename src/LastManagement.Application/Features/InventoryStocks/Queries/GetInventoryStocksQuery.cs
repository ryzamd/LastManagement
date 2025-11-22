using LastManagement.Application.Features.InventoryStocks.Interfaces;
using System.Text;

namespace LastManagement.Application.Features.InventoryStocks.Queries;

public class GetInventoryStocksQuery
{
    private readonly IInventoryStockRepository _repository;

    public GetInventoryStocksQuery(IInventoryStockRepository repository)
    {
        _repository = repository;
    }

    public async Task<(IEnumerable<InventoryStockDto> Items, int TotalCount, string? NextCursor)> ExecuteAsync(
        int? lastId,
        int? sizeId,
        int? locationId,
        string? cursor,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var (stocks, totalCount) = await _repository.GetPagedAsync(
            lastId, sizeId, locationId, cursor, limit, cancellationToken);

        var dtos = stocks.Select(s => new InventoryStockDto
        {
            Id = s.StockId,
            LastId = s.LastId,
            SizeId = s.SizeId,
            LocationId = s.LocationId,
            QuantityGood = s.QuantityGood,
            QuantityDamaged = s.QuantityDamaged,
            QuantityReserved = s.QuantityReserved,
            AvailableQuantity = s.GetAvailableQuantity(),
            LastUpdated = s.LastUpdated,
            Version = s.Version
        }).ToList();

        string? nextCursor = null;
        if (dtos.Count == limit)
        {
            var lastItem = stocks.Last();
            nextCursor = Convert.ToBase64String(Encoding.UTF8.GetBytes(lastItem.StockId.ToString()));
        }

        return (dtos, totalCount, nextCursor);
    }
}