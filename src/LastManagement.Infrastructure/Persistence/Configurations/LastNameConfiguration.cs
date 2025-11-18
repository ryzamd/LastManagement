using LastManagement.Domain.LastNames.Entities;
using LastManagement.Domain.LastNames.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LastManagement.Infrastructure.Persistence.Configurations;

public class LastNameConfiguration : IEntityTypeConfiguration<LastName>
{
    public void Configure(EntityTypeBuilder<LastName> builder)
    {
        builder.ToTable("last_names");

        builder.HasKey(ln => ln.LastId);
        builder.Property(ln => ln.LastId)
            .HasColumnName("last_id")
            .ValueGeneratedOnAdd();

        builder.Property(ln => ln.CustomerId)
            .HasColumnName("customer_id")
            .IsRequired();

        builder.Property(ln => ln.LastCode)
            .HasColumnName("last_code")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(ln => ln.LastCode)
            .IsUnique()
            .HasDatabaseName("idx_last_names_code");

        builder.Property(ln => ln.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<LastNameStatus>(v))
            .IsRequired();

        builder.HasIndex(ln => ln.Status)
            .HasDatabaseName("idx_last_names_status");

        builder.Property(ln => ln.DiscontinueReason)
            .HasColumnName("discontinue_reason")
            .HasMaxLength(500);

        builder.Property(ln => ln.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder.Property(ln => ln.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder.Property(ln => ln.Version)
            .HasColumnName("version")
            .HasDefaultValue(1)
            .IsConcurrencyToken()
            .IsRequired();

        // Foreign key
        builder.HasIndex(ln => ln.CustomerId)
            .HasDatabaseName("idx_last_names_customer");

        // No navigation properties (FK only)
        builder.Ignore(ln => ln.DomainEvents);
    }
}