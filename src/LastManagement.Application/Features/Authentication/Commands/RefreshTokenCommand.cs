using LastManagement.Application.Common.Interfaces;
using LastManagement.Application.Common.Models;
using LastManagement.Application.Features.Authentication.DTOs;
using LastManagement.Application.Features.Authentication.Interfaces;

namespace LastManagement.Application.Features.Authentication.Commands;

public sealed record RefreshTokenCommand(string RefreshToken);

public sealed class RefreshTokenCommandHandler
{
    private readonly IAccountRepository _accountRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(
        IAccountRepository accountRepository,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LoginResponse>> HandleAsync(RefreshTokenCommand command, CancellationToken cancellationToken = default)
    {
        // 1. Find account by refresh token
        var account = await _accountRepository.GetByRefreshTokenForUpdateAsync(command.RefreshToken, cancellationToken);
        if (account == null || !account.IsActive)
        {
            return Result.Failure<LoginResponse>("Invalid or expired refresh token");
        }

        // 2. Validate refresh token
        if (!account.IsRefreshTokenValid())
        {
            return Result.Failure<LoginResponse>("Refresh token has expired");
        }

        // 3. Generate new tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(account);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        // 4. Update account with new refresh token
        account.UpdateRefreshToken(newRefreshToken, refreshTokenExpiry);
        _accountRepository.Update(account);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Return response
        var response = new LoginResponse
        {
            AccessToken = accessToken,
            ExpiresIn = _jwtTokenService.GetAccessTokenExpirationMinutes() * 60,
            RefreshToken = newRefreshToken,
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