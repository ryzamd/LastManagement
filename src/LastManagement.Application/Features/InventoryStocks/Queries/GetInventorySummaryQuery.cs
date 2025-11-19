using LastManagement.Application.Features.InventoryStocks.DTOs;
using LastManagement.Application.Features.InventoryStocks.Interfaces;

namespace LastManagement.Application.Features.InventoryStocks.Queries;

public class GetInventorySummaryQuery
{
    private readonly IInventoryStockRepository _repository;

    public GetInventorySummaryQuery(IInventoryStockRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<InventorySummaryDto>> ExecuteAsync(int? customerId, int? lastId, int? locationId, CancellationToken cancellationToken = default)
    {
        var results = await _repository.GetSummaryAsync(customerId, lastId, locationId, cancellationToken);

        return results.Select(r => new InventorySummaryDto
        {
            LastId = r.LastId,
            LastCode = r.LastCode,
            LastStatus = r.LastStatus,
            CustomerName = r.CustomerName,
            LocationName = r.LocationName,
            SizeLabel = r.SizeLabel,
            QuantityGood = r.QuantityGood,
            QuantityDamaged = r.QuantityDamaged,
            QuantityReserved = r.QuantityReserved,
            AvailableQuantity = r.AvailableQuantity
        }).ToList();
    }
}