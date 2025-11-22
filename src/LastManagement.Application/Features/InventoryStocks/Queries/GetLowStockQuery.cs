using LastManagement.Api.Constants;
using LastManagement.Api.Global.Helpers;
using LastManagement.Application.Features.InventoryStocks.DTOs;
using LastManagement.Application.Features.InventoryStocks.Interfaces;

namespace LastManagement.Application.Features.InventoryStocks.Queries;

public class GetLowStockQuery
{
    private readonly IInventoryStockRepository _repository;

    public GetLowStockQuery(IInventoryStockRepository repository)
    {
        _repository = repository;
    }

    public async Task<(IEnumerable<LowStockAlertDto> Items, LowStockSummary Summary)> ExecuteAsync(int threshold, CancellationToken cancellationToken = default)
    {
        var (results, total, critical, warning) = await _repository.GetLowStockAsync(threshold, cancellationToken);

        var items = results.Select(r => new LowStockAlertDto
        {
            StockId = r.StockId,
            LastCode = r.LastCode,
            SizeLabel = r.SizeLabel,
            LocationName = r.LocationName,
            AvailableQuantity = r.AvailableQuantity,
            Threshold = threshold,
            RecommendedRestock = CalculateRestock(r.AvailableQuantity, threshold),
            Links = new Dictionary<string, object>
            {
                { "stock", new { href = UrlHelper.FormatResourceUrl(ApiRoutes.Inventory.FULL_BY_ID_TEMPLATE, r.StockId) } },
                { "createOrder", new
                    {
                        href = ApiRoutes.PurchaseOrders.FULL_BASE,
                        method = "POST"
                    }
                }
            }
        }).ToList();

        var summary = new LowStockSummary
        {
            TotalLowStockItems = total,
            CriticalItems = critical,
            WarningItems = warning
        };

        return (items, summary);
    }

    private static int CalculateRestock(int available, int threshold)
    {
        var deficit = threshold - available;
        return deficit > 0 ? deficit * 2 : threshold;
    }
}