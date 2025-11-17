using LastManagement.Domain.InventoryStocks;

namespace LastManagement.Application.Features.InventoryStocks.Interfaces;

public interface IInventoryMovementRepository
{
    Task<InventoryMovement?> GetByIdAsync(int movementId, CancellationToken cancellationToken = default);

    Task<(IEnumerable<InventoryMovement> Items, int TotalCount)> GetPagedAsync(int? lastId, string? movementType, DateTime? fromDate, DateTime? toDate, string? cursor, int limit, CancellationToken cancellationToken = default);

    Task AddAsync(InventoryMovement movement, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}