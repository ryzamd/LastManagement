using LastManagement.Application.Features.PurchaseOrders.Interfaces;
using LastManagement.Domain.PurchaseOrders.Entities;
using Microsoft.EntityFrameworkCore;

namespace LastManagement.Infrastructure.Persistence.Repositories;

public sealed class IdempotencyService : IIdempotencyService
{
    private readonly ApplicationDbContext _context;

    public IdempotencyService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string?> CheckKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        var idempotencyKey = await _context.IdempotencyKeysRepository.AsNoTracking().FirstOrDefaultAsync(ik => ik.Key == key, cancellationToken);

        if (idempotencyKey == null || idempotencyKey.IsExpired())
        {
            return null;
        }

        return idempotencyKey.Result;
    }

    public async Task StoreResultAsync(string key, string result, DateTime expiresAt, CancellationToken cancellationToken = default)
    {
        var idempotencyKey = IdempotencyKey.Create(key, result, expiresAt);
        await _context.IdempotencyKeysRepository.AddAsync(idempotencyKey, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task CleanupExpiredKeysAsync(CancellationToken cancellationToken = default)
    {
        var expiredKeys = await _context.IdempotencyKeysRepository.Where(ik => ik.ExpiresAt < DateTime.UtcNow).ToListAsync(cancellationToken);

        _context.IdempotencyKeysRepository.RemoveRange(expiredKeys);
        await _context.SaveChangesAsync(cancellationToken);
    }
}