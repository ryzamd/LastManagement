using LastManagement.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LastManagement.Infrastructure.Persistence.Configurations;

public sealed class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id)
            .HasColumnName("location_id")
            .ValueGeneratedOnAdd();

        builder.Property(l => l.LocationCode)
            .HasColumnName("location_code")
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(l => l.LocationCode)
            .IsUnique()
            .HasDatabaseName("idx_locations_code");

        builder.Property(l => l.LocationName)
            .HasColumnName("location_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(l => l.LocationType)
            .HasColumnName("location_type")
            .HasMaxLength(30)
            .HasConversion<string>()
            .IsRequired();

        builder.HasIndex(l => l.LocationType)
            .HasDatabaseName("idx_locations_type");

        builder.Property(l => l.IsActive)
            .HasColumnName("is_active")
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(l => l.IsActive)
            .HasDatabaseName("idx_locations_active");

        builder.Property(l => l.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Ignore(l => l.UpdatedAt);
        builder.Ignore(l => l.Version);
        builder.Ignore(l => l.DomainEvents);
    }
}