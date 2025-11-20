using LastManagement.Application.Features.LastNames.Interfaces;
using LastManagement.Application.Features.LastSizes.Interfaces;
using LastManagement.Application.Features.Locations.Interfaces;
using LastManagement.Application.Features.PurchaseOrders.DTOs;
using LastManagement.Application.Features.PurchaseOrders.Interfaces;

namespace LastManagement.Application.Features.PurchaseOrders.Queries;

public sealed class GetPurchaseOrderByIdQuery
{
    private readonly IPurchaseOrderRepository _orderRepository;
    private readonly IPurchaseOrderItemRepository _itemRepository;
    private readonly ILastNameRepository _lastNameRepository;
    private readonly ILastSizeRepository _sizeRepository;
    private readonly ILocationRepository _locationRepository;

    public GetPurchaseOrderByIdQuery(
        IPurchaseOrderRepository orderRepository,
        IPurchaseOrderItemRepository itemRepository,
        ILastNameRepository lastNameRepository,
        ILastSizeRepository sizeRepository,
        ILocationRepository locationRepository)
    {
        _orderRepository = orderRepository;
        _itemRepository = itemRepository;
        _lastNameRepository = lastNameRepository;
        _sizeRepository = sizeRepository;
        _locationRepository = locationRepository;
    }

    public async Task<PurchaseOrderDto?> ExecuteAsync(int orderId, bool expandItems, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
        {
            return null;
        }

        var items = await _itemRepository.GetByOrderIdAsync(orderId, cancellationToken);
        var location = await _locationRepository.GetByIdAsync(order.LocationId, cancellationToken);

        var orderDto = new PurchaseOrderDto
        {
            Id = order.OrderId,
            OrderNumber = order.OrderNumber,
            Status = order.Status.ToString(),
            LocationId = order.LocationId,
            LocationName = location?.LocationName,
            RequestedBy = order.RequestedBy,
            Department = order.Department,
            Notes = order.Notes,
            AdminNotes = order.AdminNotes,
            CreatedAt = order.CreatedAt,
            ReviewedAt = order.ReviewedAt,
            ReviewedBy = order.ReviewedBy,
            ItemCount = items.Count(),
            TotalQuantity = items.Sum(i => i.QuantityRequested)
        };

        if (expandItems)
        {
            var itemDtos = new List<PurchaseOrderItemDto>();

            foreach (var item in items)
            {
                var lastName = await _lastNameRepository.GetByIdAsync(item.LastId, cancellationToken);
                var size = await _sizeRepository.GetByIdAsync(item.SizeId, cancellationToken);

                itemDtos.Add(new PurchaseOrderItemDto
                {
                    Id = item.ItemId,
                    LastId = item.LastId,
                    LastCode = lastName?.LastCode ?? string.Empty,
                    CustomerName = string.Empty, // Will be fetched if needed
                    SizeId = item.SizeId,
                    SizeLabel = size?.SizeLabel ?? string.Empty,
                    QuantityRequested = item.QuantityRequested
                });
            }

            orderDto.Items = itemDtos;
        }

        return orderDto;
    }
}