using LastManagement.Domain.PurchaseOrders.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LastManagement.Infrastructure.Persistence.Configurations;

public sealed class PurchaseOrderItemConfiguration : IEntityTypeConfiguration<PurchaseOrderItem>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderItem> builder)
    {
        builder.ToTable("purchase_order_items");

        builder.HasKey(poi => poi.ItemId);
        builder.Property(poi => poi.ItemId)
            .HasColumnName("item_id")
            .ValueGeneratedOnAdd();

        builder.Property(poi => poi.OrderId)
            .HasColumnName("order_id")
            .IsRequired();

        builder.Property(poi => poi.LastId)
            .HasColumnName("last_id")
            .IsRequired();

        builder.HasIndex(poi => poi.LastId)
            .HasDatabaseName("idx_purchase_order_items_last");

        builder.Property(poi => poi.SizeId)
            .HasColumnName("size_id")
            .IsRequired();

        builder.HasIndex(poi => poi.SizeId)
            .HasDatabaseName("idx_purchase_order_items_size");

        builder.Property(poi => poi.QuantityRequested)
            .HasColumnName("quantity_requested")
            .IsRequired();

        // Composite index for queries by order
        builder.HasIndex(poi => new { poi.OrderId, poi.LastId, poi.SizeId })
            .HasDatabaseName("idx_purchase_order_items_composite");

        builder.Ignore(poi => poi.DomainEvents);
    }
}