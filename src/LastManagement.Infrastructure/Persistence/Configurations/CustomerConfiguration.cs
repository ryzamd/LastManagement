using LastManagement.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LastManagement.Infrastructure.Persistence.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasColumnName("customer_id")
            .ValueGeneratedOnAdd();

        builder.Property(c => c.CustomerName)
            .HasColumnName("customer_name")
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(c => c.CustomerName)
            .HasDatabaseName("idx_customers_name");

        builder.Property(c => c.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .HasConversion<string>()
            .IsRequired()
            .HasDefaultValue(CustomerStatus.Active);

        builder.HasIndex(c => c.Status)
            .HasDatabaseName("idx_customers_status");

        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamptz")
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(c => c.Version)
            .HasColumnName("version")
            .IsConcurrencyToken()
            .IsRequired()
            .HasDefaultValue(1);

        builder.Ignore(c => c.DomainEvents);
    }
}