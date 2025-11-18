using LastManagement.Application.Features.LastModels.Interfaces;
using LastManagement.Domain.LastModels.Entities;
using LastManagement.Domain.LastModels.Enums;
using Microsoft.EntityFrameworkCore;

namespace LastManagement.Infrastructure.Persistence.Repositories;

public class LastModelRepository : ILastModelRepository
{
    private readonly ApplicationDbContext _context;

    public LastModelRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<LastModel>> GetAllAsync(string? statusFilter = null, string? orderBy = null, CancellationToken cancellationToken = default)
    {
        IQueryable<LastModel> query = _context.LastModelsRepository.AsNoTracking().Include(lm => lm.LastName);

        // Apply status filter
        if (!string.IsNullOrWhiteSpace(statusFilter) && Enum.TryParse<LastModelStatus>(statusFilter, true, out var status))
        {
            query = query.Where(lm => lm.Status == status);
        }

        // Apply ordering
        query = orderBy?.ToLower() switch
        {
            "modelcode" or "modelcode asc" => query.OrderBy(lm => lm.ModelCode),
            "modelcode desc" => query.OrderByDescending(lm => lm.ModelCode),
            "createdat" or "createdat asc" => query.OrderBy(lm => lm.CreatedAt),
            "createdat desc" => query.OrderByDescending(lm => lm.CreatedAt),
            _ => query.OrderBy(lm => lm.LastId).ThenBy(lm => lm.ModelCode)
        };

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<LastModel>> GetByLastIdAsync(int lastId, CancellationToken cancellationToken = default)
    {
        IQueryable<LastModel> query = _context.LastModelsRepository.AsNoTracking()
                                                               .Include(lm => lm.LastName)
                                                               .Where(lm => lm.LastId == lastId);

        return await query.OrderBy(lm => lm.ModelCode).ToListAsync(cancellationToken);
    }

    public async Task<LastModel?> GetByIdAsync(int lastModelId, CancellationToken cancellationToken = default)
    {
        return await _context.LastModelsRepository.AsNoTracking()
                                              .Include(lm => lm.LastName)
                                              .FirstOrDefaultAsync(lm => lm.LastModelId == lastModelId, cancellationToken);
    }

    public async Task<bool> ExistsByCompositeKeyAsync(int lastId, string modelCode, CancellationToken cancellationToken = default)
    {
        return await _context.LastModelsRepository.AsNoTracking()
                                              .AnyAsync(lm => lm.LastId == lastId && lm.ModelCode == modelCode, cancellationToken);
    }
}