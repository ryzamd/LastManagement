using LastManagement.Domain.Common;
using LastManagement.Domain.LastModels.Enums;
using LastManagement.Domain.LastNames.Entities;

namespace LastManagement.Domain.LastModels.Entities;

public class LastModel : Entity
{
    public int LastModelId { get; private set; }
    public int LastId { get; private set; }
    public string ModelCode { get; private set; } = string.Empty;
    public LastModelStatus Status { get; private set; }
    public new DateTime CreatedAt { get; private set; }

    // Navigation properties
    public LastName LastName { get; private set; } = null!;

    // Private constructor for EF Core
    private LastModel() { }

    // Factory method
    public static LastModel Create(int lastId, string modelCode)
    {
        if (lastId <= 0)
            throw new ArgumentException("Last ID must be positive", nameof(lastId));

        if (string.IsNullOrWhiteSpace(modelCode))
            throw new ArgumentException("Model code is required", nameof(modelCode));

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
            throw new ArgumentException("Model code cannot be empty", nameof(newModelCode));

        if (ModelCode == newModelCode.Trim())
            return;

        ModelCode = newModelCode.Trim();
    }
}