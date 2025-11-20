using LastManagement.Domain.PurchaseOrders.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LastManagement.Infrastructure.Persistence.Configurations;

public sealed class IdempotencyKeyConfiguration : IEntityTypeConfiguration<IdempotencyKey>
{
    public void Configure(EntityTypeBuilder<IdempotencyKey> builder)
    {
        builder.ToTable("idempotency_keys");

        builder.HasKey(ik => ik.Key);
        builder.Property(ik => ik.Key)
            .HasColumnName("key")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(ik => ik.Result)
            .HasColumnName("result")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(ik => ik.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(ik => ik.ExpiresAt)
            .HasColumnName("expires_at")
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.HasIndex(ik => ik.ExpiresAt)
            .HasDatabaseName("idx_idempotency_keys_expires_at");

        builder.Ignore(ik => ik.DomainEvents);
    }
}