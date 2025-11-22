using LastManagement.Application.Common.Interfaces;
using LastManagement.Application.Common.Models;
using LastManagement.Application.Constants;
using LastManagement.Application.Features.Authentication.Interfaces;

namespace LastManagement.Application.Features.Authentication.Commands;

public sealed record LogoutCommand(int UserId);

public sealed class LogoutCommandHandler
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LogoutCommandHandler(
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(LogoutCommand command, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (account == null)
        {
            return Result.Failure(ErrorMessages.Account.ACCOUT_NOT_FOUND);
        }

        account.RevokeRefreshToken();
        _accountRepository.Update(account);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}