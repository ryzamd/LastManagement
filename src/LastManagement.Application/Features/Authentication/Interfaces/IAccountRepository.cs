using LastManagement.Domain.Accounts;

namespace LastManagement.Application.Features.Authentication.Interfaces;

public interface IAccountRepository
{
    Task<Account?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<Account?> GetByUsernameForUpdateAsync(string username, CancellationToken cancellationToken = default); // NEW
    Task<Account?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Account?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<Account?> GetByRefreshTokenForUpdateAsync(string refreshToken, CancellationToken cancellationToken = default); // NEW
    Task AddAsync(Account account, CancellationToken cancellationToken = default);
    void Update(Account account);
}