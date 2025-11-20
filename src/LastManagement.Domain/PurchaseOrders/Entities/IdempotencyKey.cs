using LastManagement.Domain.Common;

namespace LastManagement.Domain.PurchaseOrders.Entities;

public sealed class IdempotencyKey : Entity
{
    public string Key { get; private set; } = string.Empty;
    public string Result { get; private set; } = string.Empty;
    public new DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }

    private IdempotencyKey() { } // EF Core

    public static IdempotencyKey Create(string key, string result, DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be empty", nameof(key));

        if (string.IsNullOrWhiteSpace(result))
            throw new ArgumentException("Result cannot be empty", nameof(result));

        if (expiresAt <= DateTime.UtcNow)
            throw new ArgumentException("Expiration must be in the future", nameof(expiresAt));

        return new IdempotencyKey
        {
            Key = key,
            Result = result,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt
        };
    }

    public bool IsExpired() => DateTime.UtcNow > ExpiresAt;

    //public override void IncrementVersion()
    //{
    //    // Idempotency keys are immutable
    //}
}