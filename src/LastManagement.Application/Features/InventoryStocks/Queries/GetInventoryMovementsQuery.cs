using LastManagement.Application.Features.InventoryStocks.DTOs;
using LastManagement.Application.Features.InventoryStocks.Interfaces;

namespace LastManagement.Application.Features.InventoryStocks.Queries;

public class GetInventoryMovementsQuery
{
    private readonly IInventoryMovementRepository _repository;

    public GetInventoryMovementsQuery(IInventoryMovementRepository repository)
    {
        _repository = repository;
    }

    public async Task<(IEnumerable<InventoryMovementDto> Items, int TotalCount, string? NextCursor)> ExecuteAsync(int? lastId, string? movementType, DateTime? fromDate, DateTime? toDate, string? cursor, int limit, CancellationToken cancellationToken = default)
    {
        var (movements, totalCount) = await _repository.GetPagedAsync(lastId, movementType, fromDate, toDate, cursor, limit, cancellationToken);

        var movementsList = movements.ToList();

        if (!movementsList.Any())
        {
            return (Enumerable.Empty<InventoryMovementDto>(), 0, null);
        }

        // Get IDs for lookups
        var lastIds = movementsList.Select(m => m.LastId).Distinct();
        var sizeIds = movementsList.Select(m => m.SizeId).Distinct();
        var locationIds = movementsList
            .SelectMany(m => new[] { m.FromLocationId, m.ToLocationId })
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct();

        // Get lookup maps via repository
        var lasts = await _repository.GetLastNamesMapAsync(lastIds, cancellationToken);
        var sizes = await _repository.GetSizesMapAsync(sizeIds, cancellationToken);
        var locations = await _repository.GetLocationsMapAsync(locationIds, cancellationToken);

        var dtos = movementsList.Select(m => new InventoryMovementDto
        {
            Id = m.MovementId,
            LastId = m.LastId,
            LastCode = lasts.GetValueOrDefault(m.LastId, "UNKNOWN"),
            SizeId = m.SizeId,
            SizeLabel = sizes.GetValueOrDefault(m.SizeId, "UNKNOWN"),
            FromLocationId = m.FromLocationId,
            FromLocationName = m.FromLocationId.HasValue ? locations!.GetValueOrDefault(m.FromLocationId.Value, null) : null,
            ToLocationId = m.ToLocationId,
            ToLocationName = m.ToLocationId.HasValue ? locations!.GetValueOrDefault(m.ToLocationId.Value, null) : null,
            MovementType = m.MovementType,
            Quantity = m.Quantity,
            Reason = m.Reason,
            ReferenceNumber = m.ReferenceNumber,
            CreatedAt = m.CreatedAt,
            CreatedBy = m.CreatedBy,
            Links = new Dictionary<string, object>
            {
                { "self", new { href = $"/api/v1/inventory/movements/{m.MovementId}" } },
                { "last", new { href = $"/api/v1/last-names/{m.LastId}" } }
            }
        }).ToList();

        string? nextCursor = null;
        if (dtos.Count == limit)
        {
            var lastItem = movementsList.Last();
            nextCursor = Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes($"{lastItem.MovementId}"));
        }

        return (dtos, totalCount, nextCursor);
    }
}