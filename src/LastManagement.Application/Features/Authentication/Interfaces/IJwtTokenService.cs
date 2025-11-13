using LastManagement.Domain.Accounts;

namespace LastManagement.Application.Features.Authentication.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(Account account);
    string GenerateRefreshToken();
    int GetAccessTokenExpirationMinutes();
}