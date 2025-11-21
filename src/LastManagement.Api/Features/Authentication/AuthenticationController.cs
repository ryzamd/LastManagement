using Asp.Versioning;
using LastManagement.Api.Constants;
using LastManagement.Api.Global.Extensions;
using LastManagement.Application.Common.Interfaces;
using LastManagement.Application.Features.Authentication.Commands;
using LastManagement.Application.Features.Authentication.DTOs;
using LastManagement.Application.Features.Authentication.Interfaces;
using LastManagement.Application.Features.Authentication.Queries;
using LastManagement.Domain.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LastManagement.Api.Features.Authentication;

[ApiController]
[Route(ApiRoutes.Authentication.BASE)]
[ApiVersion(ApiRoutes.API_VERSION)]
public sealed class AuthenticationController : ControllerBase
{
    private readonly LoginCommandHandler _loginHandler;
    private readonly RefreshTokenCommandHandler _refreshHandler;
    private readonly LogoutCommandHandler _logoutHandler;
    private readonly GetCurrentUserQueryHandler _getCurrentUserHandler;
    private readonly ICurrentUserService _currentUserService;

    private readonly IWebHostEnvironment _environment;
    private readonly IAccountRepository _accountRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public AuthenticationController(
        LoginCommandHandler loginHandler,
        RefreshTokenCommandHandler refreshHandler,
        LogoutCommandHandler logoutHandler,
        GetCurrentUserQueryHandler getCurrentUserHandler,
        ICurrentUserService currentUserService,
        IWebHostEnvironment environment,
        IAccountRepository accountRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _loginHandler = loginHandler;
        _refreshHandler = refreshHandler;
        _logoutHandler = logoutHandler;
        _getCurrentUserHandler = getCurrentUserHandler;
        _currentUserService = currentUserService;
        _environment = environment;
        _accountRepository = accountRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Authenticate admin user and receive JWT tokens
    /// </summary>
    [HttpPost(ApiRoutes.Authentication.LOGIN)]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request.Username, request.Password);
        var result = await _loginHandler.HandleAsync(command, cancellationToken);

        return result.ToActionResult(this);
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    [HttpPost(ApiRoutes.Authentication.REFRESH)]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var command = new RefreshTokenCommand(request.RefreshToken);
        var result = await _refreshHandler.HandleAsync(command, cancellationToken);

        return result.ToActionResult(this);
    }

    /// <summary>
    /// Invalidate refresh token (logout)
    /// </summary>
    [HttpPost(ApiRoutes.Authentication.LOGOUT)]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var command = new LogoutCommand(userId.Value);
        var result = await _logoutHandler.HandleAsync(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    /// <summary>
    /// Get current authenticated user information
    /// </summary>
    [HttpGet(ApiRoutes.Authentication.ME)]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var query = new GetCurrentUserQuery(userId.Value);
        var result = await _getCurrentUserHandler.HandleAsync(query, cancellationToken);

        return result.ToActionResult(this);
    }

    /// <summary>
    /// [DEV ONLY] Create admin account
    /// </summary>
    [HttpPost(ApiRoutes.Authentication.CREATE_ADMIN)]
    [AllowAnonymous]
#if !DEBUG
    [ApiExplorerSettings(IgnoreApi = true)]
#endif
    public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminRequest request, CancellationToken cancellationToken)
    {
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        var existingAccount = await _accountRepository.GetByUsernameAsync(request.Username, cancellationToken);
        if (existingAccount != null)
        {
            return Conflict(ApiMessage.Errors.Authenication.USER_NAME_CONFLICT);
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var account = Account.Create(request.Username, passwordHash, request.FullName, AccountRole.Admin);

        await _accountRepository.AddAsync(account, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Ok(new { message = ApiMessage.Success.Authenication.CREATE_ADMIN_ACCOUNT_SUCCESS, username = account.Username });
    }
}