namespace LastManagement.Application.Features.PurchaseOrders.Interfaces;

public interface IIdempotencyService
{
    Task<string?> CheckKeyAsync(string key, CancellationToken cancellationToken = default);
    Task StoreResultAsync(string key, string result, DateTime expiresAt, CancellationToken cancellationToken = default);
    Task CleanupExpiredKeysAsync(CancellationToken cancellationToken = default);
}