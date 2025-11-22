using LastManagement.Domain.Common;
using LastManagement.Domain.Constants;
using LastManagement.Domain.LastSizes.Enums;
using LastManagement.Domain.LastSizes.Events;

namespace LastManagement.Domain.LastSizes;

public class LastSize : Entity
{
    public int SizeId { get; private set; }
    public decimal SizeValue { get; private set; }
    public string SizeLabel { get; private set; } = string.Empty;
    public SizeStatus Status { get; private set; }
    public int? ReplacementSizeId { get; private set; }
    public new DateTime CreatedAt { get; private set; }
    public new DateTime UpdatedAt { get; private set; }

    // Navigation property
    public LastSize? ReplacementSize { get; private set; }

    private LastSize() { } // EF Core

    public static LastSize Create(decimal sizeValue, string sizeLabel)
    {
        if (sizeValue <= 0)
            throw new ArgumentException(DomainValidationMessages.LastSize.VALUE_GREATER_THAN_ZERO, nameof(sizeValue));

        if (string.IsNullOrWhiteSpace(sizeLabel))
            throw new ArgumentException(DomainValidationMessages.LastSize.LABEL_REQUIRED, nameof(sizeLabel));

        if (sizeLabel.Length > 20)
            throw new ArgumentException(DomainValidationMessages.LastSize.LABEL_EXCEEDS_LENGTH, nameof(sizeLabel));

        var lastSize = new LastSize
        {
            SizeValue = sizeValue,
            SizeLabel = sizeLabel.Trim(),
            Status = SizeStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        lastSize.AddDomainEvent(new LastSizeCreatedEvent(lastSize.SizeId, lastSize.SizeValue, lastSize.SizeLabel));

        return lastSize;
    }

    public void UpdateLabel(string newLabel)
    {
        if (string.IsNullOrWhiteSpace(newLabel))
            throw new ArgumentException(DomainValidationMessages.LastSize.LABEL_REQUIRED, nameof(newLabel));

        if (newLabel.Length > 20)
            throw new ArgumentException(DomainValidationMessages.LastSize.LABEL_EXCEEDS_LENGTH, nameof(newLabel));

        SizeLabel = newLabel.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Discontinue(int? replacementSizeId = null)
    {
        if (Status == SizeStatus.Discontinued)
            throw new InvalidOperationException(DomainValidationMessages.LastSize.ALREADY_DISCONTINUED);

        Status = replacementSizeId.HasValue ? SizeStatus.Replaced : SizeStatus.Discontinued;
        ReplacementSizeId = replacementSizeId;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new LastSizeDiscontinuedEvent(SizeId, replacementSizeId));
    }

    public void Reactivate()
    {
        if (Status == SizeStatus.Active)
            throw new InvalidOperationException(DomainValidationMessages.LastSize.ALREADY_ACTIVE);

        Status = SizeStatus.Active;
        ReplacementSizeId = null;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new LastSizeReactivatedEvent(SizeId));
    }
}