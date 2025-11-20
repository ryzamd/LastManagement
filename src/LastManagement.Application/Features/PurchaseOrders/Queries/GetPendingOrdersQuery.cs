using LastManagement.Application.Features.Locations.Interfaces;
using LastManagement.Application.Features.PurchaseOrders.DTOs;
using LastManagement.Application.Features.PurchaseOrders.Interfaces;

namespace LastManagement.Application.Features.PurchaseOrders.Queries;

public sealed class GetPendingOrdersQuery
{
    private readonly IPurchaseOrderRepository _orderRepository;
    private readonly IPurchaseOrderItemRepository _itemRepository;
    private readonly ILocationRepository _locationRepository;

    public GetPendingOrdersQuery(
        IPurchaseOrderRepository orderRepository,
        IPurchaseOrderItemRepository itemRepository,
        ILocationRepository locationRepository)
    {
        _orderRepository = orderRepository;
        _itemRepository = itemRepository;
        _locationRepository = locationRepository;
    }

    public async Task<(IEnumerable<PurchaseOrderDto> Orders, PurchaseOrderSummaryDto Summary)> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var pendingOrders = await _orderRepository.GetPendingOrdersAsync(cancellationToken);
        var orderDtos = new List<PurchaseOrderDto>();

        int totalItems = 0;
        int totalQuantity = 0;

        foreach (var order in pendingOrders)
        {
            var items = await _itemRepository.GetByOrderIdAsync(order.OrderId, cancellationToken);
            var location = await _locationRepository.GetByIdAsync(order.LocationId, cancellationToken);

            var itemCount = items.Count();
            var itemQuantity = items.Sum(i => i.QuantityRequested);

            totalItems += itemCount;
            totalQuantity += itemQuantity;

            orderDtos.Add(new PurchaseOrderDto
            {
                Id = order.OrderId,
                OrderNumber = order.OrderNumber,
                Status = order.Status.ToString(),
                LocationId = order.LocationId,
                LocationName = location?.LocationName,
                RequestedBy = order.RequestedBy,
                Department = order.Department,
                CreatedAt = order.CreatedAt,
                ItemCount = itemCount,
                TotalQuantity = itemQuantity
            });
        }

        var summary = new PurchaseOrderSummaryDto
        {
            TotalPending = orderDtos.Count,
            TotalItemsRequested = totalItems,
            TotalQuantityRequested = totalQuantity
        };

        return (orderDtos, summary);
    }
}