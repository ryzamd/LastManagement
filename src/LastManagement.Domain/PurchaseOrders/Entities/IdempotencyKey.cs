using LastManagement.Domain.Common;
using LastManagement.Domain.Constants;

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
            throw new ArgumentException(DomainValidationMessages.IdempotencyKey.KEY_EMPTY, nameof(key));

        if (string.IsNullOrWhiteSpace(result))
            throw new ArgumentException(DomainValidationMessages.IdempotencyKey.RESULT_EMPTY, nameof(result));

        if (expiresAt <= DateTime.UtcNow)
            throw new ArgumentException(DomainValidationMessages.IdempotencyKey.EXPIRATION_FUTURE, nameof(expiresAt));

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