using LastManagement.Application.Constants;
using LastManagement.Application.Features.PurchaseOrders.DTOs;
using LastManagement.Application.Features.PurchaseOrders.Interfaces;
using LastManagement.Domain.PurchaseOrders.Enums;
using LastManagement.Utilities.Helpers;

namespace LastManagement.Application.Features.PurchaseOrders.Queries;

public sealed class GetPurchaseOrdersQuery
{
    private readonly IPurchaseOrderRepository _repository;
    private readonly IPurchaseOrderItemRepository _itemRepository;

    public GetPurchaseOrdersQuery(IPurchaseOrderRepository repository, IPurchaseOrderItemRepository itemRepository)
    {
        _repository = repository;
        _itemRepository = itemRepository;
    }

    public async Task<(IEnumerable<PurchaseOrderDto> Orders, int TotalCount, int? NextId)> ExecuteAsync(int limit, int? after, string? status, string? requestedBy, CancellationToken cancellationToken = default)
    {
        PurchaseOrderStatus? statusEnum = null;
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<PurchaseOrderStatus>(status, true, out var parsedStatus))
            {
                throw new ArgumentException(StringFormatter.FormatMessage(ErrorMessages.PurchaseOrder.INVALID_STATUS, status));
            }
            statusEnum = parsedStatus;
        }

        var (orders, totalCount, nextId) = await _repository.GetOrdersAsync(
            limit,
            after,
            statusEnum,
            requestedBy,
            cancellationToken);

        var orderDtos = new List<PurchaseOrderDto>();

        foreach (var order in orders)
        {
            var items = await _itemRepository.GetByOrderIdAsync(order.OrderId, cancellationToken);

            orderDtos.Add(new PurchaseOrderDto
            {
                Id = order.OrderId,
                OrderNumber = order.OrderNumber,
                Status = order.Status.ToString(),
                LocationId = order.LocationId,
                RequestedBy = order.RequestedBy,
                Department = order.Department,
                Notes = order.Notes,
                CreatedAt = order.CreatedAt,
                ReviewedAt = order.ReviewedAt,
                ReviewedBy = order.ReviewedBy,
                ItemCount = items.Count(),
                TotalQuantity = items.Sum(i => i.QuantityRequested)
            });
        }

        return (orderDtos, totalCount, nextId);
    }
}