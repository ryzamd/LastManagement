using LastManagement.Domain.Common;
using LastManagement.Domain.Constants;
using LastManagement.Domain.LastModels.Enums;
using LastManagement.Domain.LastNames.Entities;

namespace LastManagement.Domain.LastModels.Entities;

public class LastModel : Entity
{
    public int LastModelId { get; private set; }
    public int LastId { get; private set; }
    public string ModelCode { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public LastModelStatus Status { get; private set; }
    public new DateTime CreatedAt { get; private set; }

    // Navigation properties
    public LastName LastName { get; private set; } = null!;

    private LastModel() { } // EF Core

    // Factory method
    public static LastModel Create(int lastId, string modelCode)
    {
        if (lastId <= 0)
            throw new ArgumentException(DomainValidationMessages.LastModel.LAST_ID_POSITIVE, nameof(lastId));

        if (string.IsNullOrWhiteSpace(modelCode))
            throw new ArgumentException(DomainValidationMessages.LastModel.CODE_REQUIRED, nameof(modelCode));

        return new LastModel
        {
            LastId = lastId,
            ModelCode = modelCode.Trim(),
            Status = LastModelStatus.Active,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateStatus(LastModelStatus newStatus)
    {
        if (Status == newStatus)
            return;

        Status = newStatus;
    }

    public void UpdateModelCode(string newModelCode)
    {
        if (string.IsNullOrWhiteSpace(newModelCode))
            throw new ArgumentException(DomainValidationMessages.LastModel.CODE_EMPTY, nameof(newModelCode));

        if (ModelCode == newModelCode.Trim())
            return;

        ModelCode = newModelCode.Trim();
    }

    public void UpdateDescription(string? newDescription)
    {
        Description = newDescription;
        UpdatedAt = DateTime.UtcNow;
        Version++;
    }
}