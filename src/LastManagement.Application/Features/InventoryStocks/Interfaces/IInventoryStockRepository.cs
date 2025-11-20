using LastManagement.Application.Features.InventoryStocks.DTOs;
using LastManagement.Domain.InventoryStocks;
using Microsoft.EntityFrameworkCore.Storage;

namespace LastManagement.Application.Features.InventoryStocks.Interfaces;

public interface IInventoryStockRepository
{
    Task<InventoryStock?> GetByIdAsync(int stockId, CancellationToken cancellationToken = default);

    Task<InventoryStock?> GetByCompositeKeyAsync(int lastId, int sizeId, int locationId, CancellationToken cancellationToken = default);

    Task<(IEnumerable<InventoryStock> Items, int TotalCount)> GetPagedAsync(int? lastId, int? sizeId, int? locationId, string? cursor, int limit, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(int lastId, int sizeId, int locationId, CancellationToken cancellationToken = default);

    Task AddAsync(InventoryStock stock, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<InventorySummaryRaw>> GetSummaryAsync(int? customerId, int? lastId, int? locationId, CancellationToken cancellationToken = default);

    Task<(IEnumerable<LowStockRaw> Items, int TotalCount, int CriticalCount, int WarningCount)> GetLowStockAsync(int threshold, CancellationToken cancellationToken = default);

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task<InventoryStock?> GetByKeysAsync(int lastId, int sizeId, int locationId, CancellationToken cancellationToken = default);

    Task CreateAsync(InventoryStock stock, CancellationToken cancellationToken = default);

    Task UpdateAsync(InventoryStock stock, CancellationToken cancellationToken = default);
}