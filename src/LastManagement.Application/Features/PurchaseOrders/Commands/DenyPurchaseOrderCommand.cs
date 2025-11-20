using LastManagement.Application.Common.Interfaces;
using LastManagement.Application.Features.PurchaseOrders.DTOs;
using LastManagement.Application.Features.PurchaseOrders.Interfaces;

namespace LastManagement.Application.Features.PurchaseOrders.Commands;

public sealed class DenyPurchaseOrderCommand
{
    private readonly IPurchaseOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DenyPurchaseOrderCommand(IPurchaseOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PurchaseOrderDto> ExecuteAsync(int orderId, string reviewedBy, DenyOrderRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdForUpdateAsync(orderId, cancellationToken);
        if (order == null)
        {
            throw new InvalidOperationException($"Purchase order with ID {orderId} not found");
        }

        // Deny order (validates status is Pending)
        order.Deny(reviewedBy, request.AdminNotes);
        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new PurchaseOrderDto
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
            ItemCount = 0,
            TotalQuantity = 0
        };
    }
}