using LastManagement.Domain.LastModels.Entities;
using LastManagement.Domain.LastModels.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LastManagement.Infrastructure.Persistence.Configurations;

public class LastModelConfiguration : IEntityTypeConfiguration<LastModel>
{
    public void Configure(EntityTypeBuilder<LastModel> builder)
    {
        // Table name
        builder.ToTable("last_models");

        // Primary key
        builder.HasKey(lm => lm.LastModelId);
        builder.Property(lm => lm.LastModelId)
            .HasColumnName("last_model_id")
            .ValueGeneratedOnAdd();

        // Ignore base Entity.Id
        builder.Ignore(e => e.Id);
        builder.Ignore(e => e.Version);
        builder.Ignore(e => e.UpdatedAt);

        // Properties
        builder.Property(lm => lm.LastId)
            .HasColumnName("last_id")
            .IsRequired();

        builder.Property(lm => lm.ModelCode)
            .HasColumnName("model_code")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(lm => lm.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<LastModelStatus>(v))
            .HasDefaultValue(LastModelStatus.Active);

        builder.Property(lm => lm.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()");

        // Indexes
        builder.HasIndex(lm => lm.LastId)
            .HasDatabaseName("idx_last_models_last_id");

        builder.HasIndex(lm => lm.Status)
            .HasDatabaseName("idx_last_models_status");

        // Unique constraint
        builder.HasIndex(lm => new { lm.LastId, lm.ModelCode })
            .IsUnique()
            .HasDatabaseName("uq_last_models_composite");

        // Relationships
        builder.HasOne(lm => lm.LastName)
            .WithMany()
            .HasForeignKey(lm => lm.LastId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}