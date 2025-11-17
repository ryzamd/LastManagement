using LastManagement.Domain.Common;
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
            throw new ArgumentException("Size value must be greater than zero", nameof(sizeValue));

        if (string.IsNullOrWhiteSpace(sizeLabel))
            throw new ArgumentException("Size label is required", nameof(sizeLabel));

        if (sizeLabel.Length > 20)
            throw new ArgumentException("Size label cannot exceed 20 characters", nameof(sizeLabel));

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
            throw new ArgumentException("Size label is required", nameof(newLabel));

        if (newLabel.Length > 20)
            throw new ArgumentException("Size label cannot exceed 20 characters", nameof(newLabel));

        SizeLabel = newLabel.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Discontinue(int? replacementSizeId = null)
    {
        if (Status == SizeStatus.Discontinued)
            throw new InvalidOperationException("Size is already discontinued");

        Status = replacementSizeId.HasValue ? SizeStatus.Replaced : SizeStatus.Discontinued;
        ReplacementSizeId = replacementSizeId;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new LastSizeDiscontinuedEvent(SizeId, replacementSizeId));
    }

    public void Reactivate()
    {
        if (Status == SizeStatus.Active)
            throw new InvalidOperationException("Size is already active");

        Status = SizeStatus.Active;
        ReplacementSizeId = null;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new LastSizeReactivatedEvent(SizeId));
    }
}