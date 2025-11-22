using LastManagement.Domain.Accounts.Events;
using LastManagement.Domain.Common;
using LastManagement.Domain.Constants;

namespace LastManagement.Domain.Accounts;

public sealed class Account : Entity
{
    private Account() { } // EF Core

    private Account(string username, string passwordHash, string fullName, AccountRole role)
    {
        Username = username;
        PasswordHash = passwordHash;
        FullName = fullName;
        Role = role;
        IsActive = true;

        AddDomainEvent(new AccountCreatedEvent(username, fullName));
    }

    public string Username { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public AccountRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiresAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    public static Account Create(string username, string passwordHash, string fullName, AccountRole role = AccountRole.Admin)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException(DomainValidationMessages.Account.USERNAME_EMPTY, nameof(username));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException(DomainValidationMessages.Account.PASSWORD_HASH_EMPTY, nameof(passwordHash));

        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException(DomainValidationMessages.Account.FULL_NAME_EMPTY, nameof(fullName));

        return new Account(username, passwordHash, fullName, role);
    }

    public void UpdateRefreshToken(string refreshToken, DateTime expiresAt)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiresAt = expiresAt;
        IncrementVersion();
    }

    public void RevokeRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiresAt = null;
        IncrementVersion();
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        AddDomainEvent(new AccountLoginEvent(Username, LastLoginAt.Value));
        IncrementVersion();
    }

    public void Deactivate()
    {
        IsActive = false;
        RevokeRefreshToken();
        IncrementVersion();
    }

    public void UpdatePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        RevokeRefreshToken(); // Force re-login
        IncrementVersion();
    }

    public bool IsRefreshTokenValid()
    {
        return !string.IsNullOrEmpty(RefreshToken)
               && RefreshTokenExpiresAt.HasValue
               && RefreshTokenExpiresAt.Value > DateTime.UtcNow;
    }
}