using LastManagement.Domain.InventoryStocks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LastManagement.Infrastructure.Persistence.Configurations;

public class InventoryMovementConfiguration : IEntityTypeConfiguration<InventoryMovement>
{
    public void Configure(EntityTypeBuilder<InventoryMovement> builder)
    {
        builder.ToTable("inventory_movements");

        builder.HasKey(m => m.MovementId);
        builder.Property(m => m.MovementId)
            .HasColumnName("movement_id")
            .ValueGeneratedOnAdd();

        builder.Property(m => m.LastId)
            .HasColumnName("last_id")
            .IsRequired();

        builder.Property(m => m.SizeId)
            .HasColumnName("size_id")
            .IsRequired();

        builder.Property(m => m.FromLocationId)
            .HasColumnName("from_location_id");

        builder.Property(m => m.ToLocationId)
            .HasColumnName("to_location_id");

        builder.Property(m => m.MovementType)
            .HasColumnName("movement_type")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(m => m.Quantity)
            .HasColumnName("quantity")
            .IsRequired();

        builder.Property(m => m.Reason)
            .HasColumnName("reason")
            .HasColumnType("text");

        builder.Property(m => m.ReferenceNumber)
            .HasColumnName("reference_number")
            .HasMaxLength(50);

        builder.Property(m => m.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()");

        builder.Property(m => m.CreatedBy)
            .HasColumnName("created_by")
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(m => new { m.LastId, m.CreatedAt })
            .HasDatabaseName("idx_movements_last");

        builder.HasIndex(m => new { m.MovementType, m.CreatedAt })
            .HasDatabaseName("idx_movements_type");

        builder.HasIndex(m => m.ReferenceNumber)
            .HasDatabaseName("idx_movements_reference")
            .HasFilter("reference_number IS NOT NULL");

        // Ignore domain events
        builder.Ignore(m => m.DomainEvents);
    }
}