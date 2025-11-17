using LastManagement.Domain.LastSizes;
using LastManagement.Domain.LastSizes.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LastManagement.Infrastructure.Persistence.Configurations;

public class LastSizeConfiguration : IEntityTypeConfiguration<LastSize>
{
    public void Configure(EntityTypeBuilder<LastSize> builder)
    {
        builder.ToTable("last_sizes");

        builder.HasKey(ls => ls.SizeId);

        builder.Property(ls => ls.SizeId)
            .HasColumnName("size_id")
            .ValueGeneratedOnAdd();

        builder.Property(ls => ls.SizeValue)
            .HasColumnName("size_value")
            .HasPrecision(4, 1)
            .IsRequired();

        builder.Property(ls => ls.SizeLabel)
            .HasColumnName("size_label")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(ls => ls.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<SizeStatus>(v))
            .HasDefaultValue(SizeStatus.Active)
            .IsRequired();

        builder.Property(ls => ls.ReplacementSizeId)
            .HasColumnName("replacement_size_id");

        builder.Property(ls => ls.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder.Property(ls => ls.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        // Indexes
        builder.HasIndex(ls => ls.SizeValue)
            .IsUnique()
            .HasDatabaseName("idx_last_sizes_value");

        builder.HasIndex(ls => ls.Status)
            .HasDatabaseName("idx_last_sizes_status");

        // Self-referencing relationship
        builder.HasOne(ls => ls.ReplacementSize)
            .WithMany()
            .HasForeignKey(ls => ls.ReplacementSizeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ignore domain events
        builder.Ignore(ls => ls.DomainEvents);
    }
}