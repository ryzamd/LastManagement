using LastManagement.Domain.PurchaseOrders.Entities;
using LastManagement.Domain.PurchaseOrders.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LastManagement.Infrastructure.Persistence.Configurations;

public sealed class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.ToTable("purchase_orders");

        builder.HasKey(po => po.OrderId);
        builder.Property(po => po.OrderId)
            .HasColumnName("order_id")
            .ValueGeneratedOnAdd();

        builder.Property(po => po.OrderNumber)
            .HasColumnName("order_number")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(po => po.OrderNumber)
            .IsUnique()
            .HasDatabaseName("uq_purchase_orders_number");

        builder.Property(po => po.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .HasConversion<string>()
            .IsRequired()
            .HasDefaultValue(PurchaseOrderStatus.Pending);

        builder.HasIndex(po => po.Status)
            .HasDatabaseName("idx_purchase_orders_status");

        builder.Property(po => po.LocationId)
            .HasColumnName("location_id")
            .IsRequired();

        builder.HasIndex(po => po.LocationId)
            .HasDatabaseName("idx_purchase_orders_location");

        builder.Property(po => po.RequestedBy)
            .HasColumnName("requested_by")
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(po => po.RequestedBy)
            .HasDatabaseName("idx_purchase_orders_requested_by");

        builder.Property(po => po.Department)
            .HasColumnName("department")
            .HasMaxLength(200);

        builder.Property(po => po.Notes)
            .HasColumnName("notes")
            .HasMaxLength(1000);

        builder.Property(po => po.AdminNotes)
            .HasColumnName("admin_notes")
            .HasMaxLength(1000);

        builder.Property(po => po.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.HasIndex(po => po.CreatedAt)
            .HasDatabaseName("idx_purchase_orders_created_at");

        builder.Property(po => po.ReviewedAt)
            .HasColumnName("reviewed_at")
            .HasColumnType("timestamptz");

        builder.Property(po => po.ReviewedBy)
            .HasColumnName("reviewed_by")
            .HasMaxLength(200);

        builder.Property(po => po.Version)
            .HasColumnName("version")
            .IsConcurrencyToken()
            .IsRequired()
            .HasDefaultValue(1);

        // Relationships
        builder.HasMany(po => po.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(po => po.DomainEvents);
    }
}