using LastManagement.Domain.Common;
using LastManagement.Domain.Constants;
using LastManagement.Domain.Locations.Events;

namespace LastManagement.Domain.Locations;

public sealed class Location : Entity
{
    private Location() { } // EF Core

    private Location(string locationCode, string locationName, LocationType locationType)
    {
        LocationCode = locationCode;
        LocationName = locationName;
        LocationType = locationType;
        IsActive = true;
        AddDomainEvent(new LocationCreatedEvent(locationCode, locationName));
    }

    public string LocationCode { get; private set; } = string.Empty;
    public string LocationName { get; private set; } = string.Empty;
    public LocationType LocationType { get; private set; }
    public bool IsActive { get; private set; }

    public static Location Create(string locationCode, string locationName, LocationType locationType)
    {
        if (string.IsNullOrWhiteSpace(locationCode))
            throw new ArgumentException(DomainValidationMessages.Location.CODE_EMPTY, nameof(locationCode));

        if (locationCode.Length > 20)
            throw new ArgumentException(DomainValidationMessages.Location.CODE_EXCEEDS_LENGTH, nameof(locationCode));

        if (string.IsNullOrWhiteSpace(locationName))
            throw new ArgumentException(DomainValidationMessages.Location.NAME_EMPTY, nameof(locationName));

        if (locationName.Length > 100)
            throw new ArgumentException(DomainValidationMessages.Location.NAME_EXCEEDS_LENGTH, nameof(locationName));

        return new Location(locationCode, locationName, locationType);
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException(DomainValidationMessages.Location.NAME_EMPTY, nameof(newName));

        if (newName.Length > 100)
            throw new ArgumentException(DomainValidationMessages.Location.NAME_EXCEEDS_LENGTH, nameof(newName));

        LocationName = newName;
        IncrementVersion();
    }

    public void UpdateType(LocationType newType)
    {
        if (LocationType == newType) return;

        LocationType = newType;
        IncrementVersion();
    }

    public void Activate()
    {
        if (IsActive) return;
        IsActive = true;
        IncrementVersion();
    }

    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
        IncrementVersion();
        AddDomainEvent(new LocationDeactivatedEvent(Id, LocationCode));
    }
}