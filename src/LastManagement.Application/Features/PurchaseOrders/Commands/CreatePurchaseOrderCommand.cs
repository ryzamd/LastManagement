using LastManagement.Application.Common.Interfaces;
using LastManagement.Application.Constants;
using LastManagement.Application.Features.LastNames.Interfaces;
using LastManagement.Application.Features.LastSizes.Interfaces;
using LastManagement.Application.Features.Locations.Interfaces;
using LastManagement.Application.Features.PurchaseOrders.DTOs;
using LastManagement.Application.Features.PurchaseOrders.Interfaces;
using LastManagement.Domain.PurchaseOrders.Entities;
using LastManagement.Utilities.Helpers;

namespace LastManagement.Application.Features.PurchaseOrders.Commands;

public sealed class CreatePurchaseOrderCommand
{
    private readonly IPurchaseOrderRepository _orderRepository;
    private readonly IPurchaseOrderItemRepository _itemRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly ILastNameRepository _lastNameRepository;
    private readonly ILastSizeRepository _sizeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePurchaseOrderCommand(
        IPurchaseOrderRepository orderRepository,
        IPurchaseOrderItemRepository itemRepository,
        ILocationRepository locationRepository,
        ILastNameRepository lastNameRepository,
        ILastSizeRepository sizeRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _itemRepository = itemRepository;
        _locationRepository = locationRepository;
        _lastNameRepository = lastNameRepository;
        _sizeRepository = sizeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PurchaseOrder> ExecuteAsync(CreatePurchaseOrderRequest request, CancellationToken cancellationToken = default)
    {
        // Validate location exists
        var locationExists = await _locationRepository.ExistsAsync(request.LocationId, cancellationToken);
        if (!locationExists)
        {
            throw new InvalidOperationException(StringFormatter.FormatError(ErrorMessages.Location.DOES_NOT_EXIST, request.LocationId));
        }

        // Validate all lasts and sizes exist
        foreach (var item in request.Items)
        {
            var lastExists = await _lastNameRepository.ExistsAsync(item.LastId, cancellationToken);
            if (!lastExists)
            {
                throw new InvalidOperationException(StringFormatter.FormatError(ErrorMessages.LastName.DOES_NOT_EXIST, item.LastId));
            }

            var sizeExists = await _sizeRepository.ExistsSizeIdAsync(item.SizeId, cancellationToken);
            if (!sizeExists)
            {
                throw new InvalidOperationException(StringFormatter.FormatError(ErrorMessages.Size.DOES_NOT_EXIST, item.SizeId));
            }
        }

        // Generate order number
        var today = DateTime.UtcNow.ToString(FormatConstants.DateTime.DATE_ONLY_FORMAT);
        var lastOrderNumber = await _orderRepository.GetLastOrderNumberForDateAsync(today, cancellationToken);

        int sequence = 1;
        if (!string.IsNullOrEmpty(lastOrderNumber))
        {
            var lastSequence = int.Parse(lastOrderNumber.Split('-').Last());
            sequence = lastSequence + 1;
        }

        if (sequence > FormatConstants.PurchaseOrder.MAX_SEQUENCE_PER_DAY)
        {
            throw new InvalidOperationException(ErrorMessages.PurchaseOrder.MAX_ORDERS_EXCEEDED);
        }

        var orderNumber = StringFormatter.FormatValidation(FormatConstants.PurchaseOrder.ORDER_NUMBER_TEMPLATE, today, sequence);

        // Create order
        var order = PurchaseOrder.Create(
            orderNumber,
            request.LocationId,
            request.RequestedBy,
            request.Department,
            request.Notes);

        await _orderRepository.CreateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Create items
        var items = request.Items.Select(i =>
            PurchaseOrderItem.Create(order.OrderId, i.LastId, i.SizeId, i.QuantityRequested));

        await _itemRepository.CreateBatchAsync(items, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return order;
    }
}