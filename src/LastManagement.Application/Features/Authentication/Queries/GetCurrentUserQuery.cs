using LastManagement.Application.Common.Models;
using LastManagement.Application.Constants;
using LastManagement.Application.Features.Authentication.DTOs;
using LastManagement.Application.Features.Authentication.Interfaces;

namespace LastManagement.Application.Features.Authentication.Queries;

public sealed record GetCurrentUserQuery(int UserId);

public sealed class GetCurrentUserQueryHandler
{
    private readonly IAccountRepository _accountRepository;

    public GetCurrentUserQueryHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Result<UserDto>> HandleAsync(GetCurrentUserQuery query, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsync(query.UserId, cancellationToken);
        if (account == null || !account.IsActive)
        {
            return Result.Failure<UserDto>(ErrorMessages.Account.USER_NOT_FOUND);
        }

        var userDto = new UserDto
        {
            Id = account.Id,
            Username = account.Username,
            FullName = account.FullName,
            Role = account.Role.ToString(),
            LastLoginAt = account.LastLoginAt,
            CreatedAt = account.CreatedAt
        };

        return Result.Success(userDto);
    }
}