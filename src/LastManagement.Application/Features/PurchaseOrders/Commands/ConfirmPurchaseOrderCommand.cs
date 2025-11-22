using LastManagement.Application.Common.Interfaces;
using LastManagement.Application.Constants;
using LastManagement.Application.Features.InventoryStocks.Interfaces;
using LastManagement.Application.Features.LastNames.Interfaces;
using LastManagement.Application.Features.LastSizes.Interfaces;
using LastManagement.Application.Features.Locations.Interfaces;
using LastManagement.Application.Features.PurchaseOrders.DTOs;
using LastManagement.Application.Features.PurchaseOrders.Interfaces;
using LastManagement.Domain.InventoryStocks;
using LastManagement.Utilities.Helpers;

namespace LastManagement.Application.Features.PurchaseOrders.Commands;

public sealed class ConfirmPurchaseOrderCommand
{
    private readonly IPurchaseOrderRepository _orderRepository;
    private readonly IPurchaseOrderItemRepository _itemRepository;
    private readonly IInventoryStockRepository _stockRepository;
    private readonly IInventoryMovementRepository _movementRepository;
    private readonly ILastNameRepository _lastNameRepository;
    private readonly ILastSizeRepository _sizeRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmPurchaseOrderCommand(
        IPurchaseOrderRepository orderRepository,
        IPurchaseOrderItemRepository itemRepository,
        IInventoryStockRepository stockRepository,
        IInventoryMovementRepository movementRepository,
        ILastNameRepository lastNameRepository,
        ILastSizeRepository sizeRepository,
        ILocationRepository locationRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _itemRepository = itemRepository;
        _stockRepository = stockRepository;
        _movementRepository = movementRepository;
        _lastNameRepository = lastNameRepository;
        _sizeRepository = sizeRepository;
        _locationRepository = locationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<(PurchaseOrderDto Order, List<InventoryUpdateDto> Updates)> ExecuteAsync(int orderId, string reviewedBy, ConfirmOrderRequest request, CancellationToken cancellationToken = default)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Get order with items
            var order = await _orderRepository.GetByIdWithItemsAsync(orderId, cancellationToken);
            if (order == null)
            {
                throw new InvalidOperationException(StringFormatter.FormatMessage(ErrorMessages.PurchaseOrder.NOT_FOUND, orderId));
            }

            // Confirm order (validates status is Pending)
            order.Confirm(reviewedBy, request.AdminNotes);
            await _orderRepository.UpdateAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Get order items
            var items = await _itemRepository.GetByOrderIdAsync(orderId, cancellationToken);
            var inventoryUpdates = new List<InventoryUpdateDto>();

            // Process each item
            foreach (var item in items)
            {
                // Find or create stock
                var stock = await _stockRepository.GetByKeysAsync(
                    item.LastId,
                    item.SizeId,
                    order.LocationId,
                    cancellationToken);

                int previousQuantity = stock?.QuantityGood ?? 0;

                if (stock == null)
                {
                    stock = InventoryStock.Create(
                        item.LastId,
                        item.SizeId,
                        order.LocationId,
                        item.QuantityRequested,
                        0,
                        0);
                    await _stockRepository.CreateAsync(stock, cancellationToken);
                }
                else
                {
                    stock.AdjustQuantity(item.QuantityRequested, 0, 0);
                    await _stockRepository.UpdateAsync(stock, cancellationToken);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Create movement log
                var movement = InventoryMovement.CreatePurchase(
                    item.LastId,
                    item.SizeId,
                    order.LocationId,
                    item.QuantityRequested,
                    order.OrderNumber,
                    reviewedBy);

                await _movementRepository.CreateAsync(movement, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Get display names
                var lastName = await _lastNameRepository.GetByIdAsync(item.LastId, cancellationToken);
                var size = await _sizeRepository.GetByIdAsync(item.SizeId, cancellationToken);
                var location = await _locationRepository.GetByIdAsync(order.LocationId, cancellationToken);

                inventoryUpdates.Add(new InventoryUpdateDto
                {
                    StockId = stock.StockId,
                    LastCode = lastName?.LastCode ?? string.Empty,
                    SizeLabel = size?.SizeLabel ?? string.Empty,
                    LocationName = location?.LocationName ?? string.Empty,
                    PreviousQuantity = previousQuantity,
                    NewQuantity = stock.QuantityGood,
                    MovementId = movement.MovementId
                });
            }

            await transaction.CommitAsync(cancellationToken);

            // Build response DTO
            var orderDto = new PurchaseOrderDto
            {
                Id = order.OrderId,
                OrderNumber = order.OrderNumber,
                Status = order.Status.ToString(),
                LocationId = order.LocationId,
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

            return (orderDto, inventoryUpdates);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}