using LastManagement.Domain.Common;
using LastManagement.Domain.Constants;
using LastManagement.Domain.LastNames.Enums;

namespace LastManagement.Domain.LastNames.Entities;

public class LastName : Entity
{
    public int LastId { get; private set; }
    public int CustomerId { get; private set; }
    public string LastCode { get; private set; } = string.Empty;
    public LastNameStatus Status { get; private set; }
    public string? DiscontinueReason { get; private set; }
    public new DateTime CreatedAt { get; private set; }
    public new DateTime UpdatedAt { get; private set; }
    public new int Version { get; private set; }

    // Private constructor for EF Core
    private LastName() { }

    public static LastName Create(int customerId, string lastCode)
    {
        if (customerId <= 0)
            throw new ArgumentException(DomainValidationMessages.LastName.CUSTOMER_ID_POSITIVE, nameof(customerId));

        if (string.IsNullOrWhiteSpace(lastCode))
            throw new ArgumentException(DomainValidationMessages.LastName.CODE_EMPTY, nameof(lastCode));

        if (lastCode.Length > 50)
            throw new ArgumentException(DomainValidationMessages.LastName.CODE_EXCEEDS_LENGTH, nameof(lastCode));

        return new LastName
        {
            CustomerId = customerId,
            LastCode = lastCode.Trim(),
            Status = LastNameStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 1
        };
    }

    public void UpdateStatus(LastNameStatus newStatus, string? discontinueReason = null)
    {
        if (newStatus != LastNameStatus.Active && string.IsNullOrWhiteSpace(discontinueReason))
            throw new ArgumentException(DomainValidationMessages.LastName.DISCONTINUE_REASON_REQUIRED);


        Status = newStatus;
        DiscontinueReason = newStatus == LastNameStatus.Active ? null : discontinueReason;
        UpdatedAt = DateTime.UtcNow;
        Version++;
    }

    public void TransferToCustomer(int newCustomerId)
    {
        if (newCustomerId <= 0)
            throw new ArgumentException(DomainValidationMessages.LastName.CUSTOMER_ID_POSITIVE, nameof(newCustomerId));

        if (newCustomerId == CustomerId)
            throw new InvalidOperationException(DomainValidationMessages.LastName.CANNOT_TRANSFER_SAME_CUSTOMER);

        CustomerId = newCustomerId;
        UpdatedAt = DateTime.UtcNow;
        Version++;
    }

    public void UpdateLastCode(string newLastCode)
    {
        if (string.IsNullOrWhiteSpace(newLastCode))
            throw new ArgumentException(DomainValidationMessages.LastName.CODE_EMPTY, nameof(newLastCode));

        if (newLastCode.Length > 50)
            throw new ArgumentException(DomainValidationMessages.LastName.CODE_EXCEEDS_LENGTH, nameof(newLastCode));

        LastCode = newLastCode.Trim();
        UpdatedAt = DateTime.UtcNow;
        Version++;
    }
}