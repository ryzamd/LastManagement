using LastManagement.Domain.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LastManagement.Infrastructure.Persistence.Configurations;

public sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("accounts");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasColumnName("account_id")
            .ValueGeneratedOnAdd();

        builder.Property(a => a.Username)
            .HasColumnName("username")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(a => a.Username)
            .IsUnique()
            .HasDatabaseName("idx_accounts_username");

        builder.Property(a => a.PasswordHash)
            .HasColumnName("password_hash")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(a => a.FullName)
            .HasColumnName("full_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.Role)
            .HasColumnName("role")
            .HasMaxLength(20)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(a => a.IsActive)
            .HasColumnName("is_active")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(a => a.RefreshToken)
            .HasColumnName("refresh_token")
            .HasMaxLength(500);

        builder.HasIndex(a => a.RefreshToken)
            .HasDatabaseName("idx_accounts_refresh_token")
            .HasFilter("refresh_token IS NOT NULL");

        builder.Property(a => a.RefreshTokenExpiresAt)
            .HasColumnName("refresh_token_expires_at")
            .HasColumnType("timestamptz");

        builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(a => a.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamptz")
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(a => a.LastLoginAt)
            .HasColumnName("last_login_at")
            .HasColumnType("timestamptz");

        builder.HasIndex(a => a.LastLoginAt)
            .HasDatabaseName("idx_accounts_last_login");

        builder.Property(a => a.Version)
            .HasColumnName("version")
            .IsConcurrencyToken()
            .IsRequired();
        //.HasDefaultValue(1);

        // Ignore domain events (not persisted)
        builder.Ignore(a => a.DomainEvents);
    }
}