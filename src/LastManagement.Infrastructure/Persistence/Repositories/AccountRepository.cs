using LastManagement.Application.Features.Authentication.Interfaces;
using LastManagement.Domain.Accounts;
using Microsoft.EntityFrameworkCore;

namespace LastManagement.Infrastructure.Persistence.Repositories;

public sealed class AccountRepository : IAccountRepository
{
    private readonly ApplicationDbContext _context;

    public AccountRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Account?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            //.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Username == username, cancellationToken);
    }

    public async Task<Account?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<Account?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.RefreshToken == refreshToken, cancellationToken);
    }

    public async Task AddAsync(Account account, CancellationToken cancellationToken = default)
    {
        await _context.Accounts.AddAsync(account, cancellationToken);
    }

    public void Update(Account account)
    {
        _context.Accounts.Update(account);
    }

    public async Task<Account?> GetByUsernameForUpdateAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .FirstOrDefaultAsync(a => a.Username == username, cancellationToken);
    }

    public async Task<Account?> GetByRefreshTokenForUpdateAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .FirstOrDefaultAsync(a => a.RefreshToken == refreshToken, cancellationToken);
    }
}