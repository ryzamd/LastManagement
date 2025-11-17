using LastManagement.Domain.InventoryStocks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LastManagement.Infrastructure.Persistence.Configurations;

public class InventoryStockConfiguration : IEntityTypeConfiguration<InventoryStock>
{
    public void Configure(EntityTypeBuilder<InventoryStock> builder)
    {
        builder.ToTable("inventory_stocks");

        builder.HasKey(s => s.StockId);
        builder.Property(s => s.StockId)
            .HasColumnName("stock_id")
            .ValueGeneratedOnAdd();

        builder.Property(s => s.LastId)
            .HasColumnName("last_id")
            .IsRequired();

        builder.Property(s => s.SizeId)
            .HasColumnName("size_id")
            .IsRequired();

        builder.Property(s => s.LocationId)
            .HasColumnName("location_id")
            .IsRequired();

        builder.Property(s => s.QuantityGood)
            .HasColumnName("quantity_good")
            .HasDefaultValue(0);

        builder.Property(s => s.QuantityDamaged)
            .HasColumnName("quantity_damaged")
            .HasDefaultValue(0);

        builder.Property(s => s.QuantityReserved)
            .HasColumnName("quantity_reserved")
            .HasDefaultValue(0);

        builder.Property(s => s.LastUpdated)
            .HasColumnName("last_updated")
            .HasDefaultValueSql("NOW()");

        builder.Property(s => s.Version)
            .HasColumnName("version")
            .IsConcurrencyToken()
            .HasDefaultValue(1);

        // Unique constraint
        builder.HasIndex(s => new { s.LastId, s.SizeId, s.LocationId })
            .IsUnique()
            .HasDatabaseName("uq_inventory_stocks_composite");

        // Indexes
        builder.HasIndex(s => new { s.LastId, s.SizeId })
            .HasDatabaseName("idx_inventory_last_size");

        builder.HasIndex(s => s.LocationId)
            .HasDatabaseName("idx_inventory_location");

        // Ignore domain events
        builder.Ignore(s => s.DomainEvents);
    }
}