using LastManagement.Application.Common.Interfaces;
using LastManagement.Application.Common.Models;
using LastManagement.Application.Features.Authentication.DTOs;
using LastManagement.Application.Features.Authentication.Interfaces;
using LastManagement.Utilities.Constants.Global;

namespace LastManagement.Application.Features.Authentication.Commands;

public sealed record LoginCommand(string Username, string Password);

public sealed class LoginCommandHandler
{
    private readonly IAccountRepository _accountRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        IAccountRepository accountRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LoginResponse>> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        // 1. Find account
        var account = await _accountRepository.GetByUsernameForUpdateAsync(command.Username, cancellationToken);
        if (account == null || !account.IsActive)
        {
            return Result.Failure<LoginResponse>(ResultMessages.Authentication.INVALID_CREDENTIALS);
        }

        // 2. Verify password
        if (!_passwordHasher.VerifyPassword(command.Password, account.PasswordHash))
        {
            return Result.Failure<LoginResponse>(ResultMessages.Authentication.INVALID_CREDENTIALS);
        }

        // 3. Generate tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(account);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        // 4. Update account
        account.UpdateRefreshToken(refreshToken, refreshTokenExpiry);
        account.RecordLogin();
        _accountRepository.Update(account);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Return response
        var response = new LoginResponse
        {
            AccessToken = accessToken,
            ExpiresIn = _jwtTokenService.GetAccessTokenExpirationMinutes() * 60,
            RefreshToken = refreshToken,
            User = new UserDto
            {
                Id = account.Id,
                Username = account.Username,
                FullName = account.FullName,
                Role = account.Role.ToString(),
                LastLoginAt = account.LastLoginAt,
                CreatedAt = account.CreatedAt
            }
        };

        return Result.Success(response);
    }
}