using Asp.Versioning;
using LastManagement.Api.Constants;
using LastManagement.Api.Global.Constants;
using LastManagement.Api.Global.Helpers;
using LastManagement.Application.Features.Customers.Commands;
using LastManagement.Application.Features.Customers.DTOs;
using LastManagement.Application.Features.Customers.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LastManagement.Api.Features.Customers;

[ApiController]
[Route(ApiRoutes.Customers.BASE)]
[ApiVersion(ApiRoutes.API_VERSION)]
public sealed class CustomersController : ControllerBase
{
    private readonly GetCustomersQueryHandler _getCustomersHandler;
    private readonly GetCustomerByIdQueryHandler _getCustomerByIdHandler;
    private readonly CreateCustomerCommandHandler _createCustomerHandler;
    private readonly UpdateCustomerCommandHandler _updateCustomerHandler;
    private readonly DeleteCustomerCommandHandler _deleteCustomerHandler;

    public CustomersController(
        GetCustomersQueryHandler getCustomersHandler,
        GetCustomerByIdQueryHandler getCustomerByIdHandler,
        CreateCustomerCommandHandler createCustomerHandler,
        UpdateCustomerCommandHandler updateCustomerHandler,
        DeleteCustomerCommandHandler deleteCustomerHandler)
    {
        _getCustomersHandler = getCustomersHandler;
        _getCustomerByIdHandler = getCustomerByIdHandler;
        _createCustomerHandler = createCustomerHandler;
        _updateCustomerHandler = updateCustomerHandler;
        _deleteCustomerHandler = deleteCustomerHandler;
    }

    /// <summary>
    /// List all customers with pagination and filtering
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PaginatedResponse<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCustomers([FromQuery] int limit = CustomerConstants.DefaultPageSize, [FromQuery] int? after = null, [FromQuery(Name = "$filter")] string? filter = null, [FromQuery(Name = "$orderby")] string? orderby = null, CancellationToken cancellationToken = default)
    {
        // Validate limit
        if (limit < 1) limit = CustomerConstants.DefaultPageSize;
        if (limit > CustomerConstants.MaxPageSize) limit = CustomerConstants.MaxPageSize;

        // Parse filter (simplified - only status eq 'X')
        string? filterStatus = null;
        if (!string.IsNullOrEmpty(filter))
        {
            var match = System.Text.RegularExpressions.Regex.Match(filter, RegexPattern.Customer.GET_CUSTOMERS, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (match.Success)
            {
                filterStatus = match.Groups[1].Value;
            }
        }

        var query = new GetCustomersQuery(limit, after, filterStatus, orderby);
        var result = await _getCustomersHandler.HandleAsync(query, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        Response.Headers.Append("X-Total-Count", result.Value!.Count.ToString());
        Response.Headers.CacheControl = "public, max-age=60";

        return Ok(result.Value);
    }

    /// <summary>
    /// Get customer by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomerById(int id, CancellationToken cancellationToken)
    {
        var query = new GetCustomerByIdQuery(id);
        var result = await _getCustomerByIdHandler.HandleAsync(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(new ProblemDetails
            {
                Type = ProblemDetailsConstants.Types.NOT_FOUND,
                Title = ProblemDetailsConstants.Titles.NOT_FOUND,
                Status = StatusCodes.Status404NotFound,
                Detail = result.Error,
                Instance = $"/api/v1/customers/{id}"
            });
        }

        var etag = ETagHelper.Generate(result.Value!.Version);
        Response.Headers.ETag = etag;
        Response.Headers.CacheControl = "public, max-age=60";

        return Ok(result.Value);
    }

    /// <summary>
    /// Create a new customer
    /// </summary>
    [HttpPost]
    [Authorize(Roles = AuthorizationConstants.Roles.ADMIN)]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateCustomerCommand(request.CustomerName, request.Status);
        var result = await _createCustomerHandler.HandleAsync(command, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error!.Contains(ApiMessage.Errors.Customer.CUSTOMER_EXISTED))
            {
                return Conflict(new ProblemDetails
                {
                    Type = ProblemDetailsConstants.Types.DUPLICATE_RESOURCE,
                    Title = ProblemDetailsConstants.Titles.DUPLICATE_RESOURCE,
                    Status = StatusCodes.Status409Conflict,
                    Detail = result.Error,
                    Instance = "/api/v1/customers"
                });
            }
            return BadRequest(result.Error);
        }

        var etag = ETagHelper.Generate(result.Value!.Version);
        Response.Headers.ETag = etag;
        Response.Headers.Location = $"/api/v1/customers/{result.Value.Id}";

        return CreatedAtAction(
            nameof(GetCustomerById),
            new { id = result.Value.Id },
            result.Value);
    }

    /// <summary>
    /// Partial update of customer
    /// </summary>
    [HttpPatch("{id:int}")]
    [Authorize(Roles = AuthorizationConstants.Roles.ADMIN)]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
    public async Task<IActionResult> UpdateCustomer(int id, [FromBody] UpdateCustomerRequest request, [FromHeader(Name = "If-Match")] string? ifMatch, CancellationToken cancellationToken)
    {
        // Parse version from ETag
        var expectedVersion = ETagHelper.ParseVersion(ifMatch);
        if (!expectedVersion.HasValue)
        {
            return BadRequest(new ProblemDetails
            {
                Type = ProblemDetailsConstants.Types.VALIDATION_ERROR,
                Title = ProblemDetailsConstants.Titles.VALIDATION_ERROR,
                Status = StatusCodes.Status400BadRequest,
                Detail = ProblemDetailsConstants.Details.IF_MATCH_HEADER_REQUIRED_ETAG,
                Instance = $"/api/v1/customers/{id}"
            });
        }

        var command = new UpdateCustomerCommand(
            id,
            request.CustomerName,
            request.Status,
            expectedVersion.Value);

        var result = await _updateCustomerHandler.HandleAsync(command, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error == ApiMessage.Errors.Customer.CUSTOMER_NOT_FOUND)
            {
                return NotFound(new ProblemDetails
                {
                    Type = ProblemDetailsConstants.Types.NOT_FOUND,
                    Title = ProblemDetailsConstants.Titles.NOT_FOUND,
                    Status = StatusCodes.Status404NotFound,
                    Detail = result.Error,
                    Instance = $"/api/v1/customers/{id}"
                });
            }

            if (result.Error!.Contains(ApiMessage.Errors.Customer.CUSTOMER_MODIFIED_BY_ANOTHER))
            {
                return StatusCode(StatusCodes.Status412PreconditionFailed, new ProblemDetails
                {
                    Type = ProblemDetailsConstants.Types.PRECONDITION_FAILED,
                    Title = ProblemDetailsConstants.Titles.PRECONDITION_FAILED,
                    Status = StatusCodes.Status412PreconditionFailed,
                    Detail = result.Error,
                    Instance = $"/api/v1/customers/{id}"
                });
            }

            if (result.Error!.Contains(ApiMessage.Errors.Customer.CUSTOMER_EXISTED))
            {
                return Conflict(new ProblemDetails
                {
                    Type = ProblemDetailsConstants.Types.DUPLICATE_RESOURCE,
                    Title = ProblemDetailsConstants.Titles.DUPLICATE_RESOURCE,
                    Status = StatusCodes.Status409Conflict,
                    Detail = result.Error,
                    Instance = $"/api/v1/customers/{id}"
                });
            }

            return BadRequest(result.Error);
        }

        var newEtag = ETagHelper.Generate(result.Value!.Version);
        Response.Headers.ETag = newEtag;

        return Ok(result.Value);
    }

    /// <summary>
    /// Delete a customer
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = AuthorizationConstants.Roles.ADMIN)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteCustomer(int id, CancellationToken cancellationToken)
    {
        var command = new DeleteCustomerCommand(id);
        var result = await _deleteCustomerHandler.HandleAsync(command, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error == ApiMessage.Errors.Customer.CUSTOMER_NOT_FOUND)
            {
                return NotFound(new ProblemDetails
                {
                    Type = ProblemDetailsConstants.Types.NOT_FOUND,
                    Title = ProblemDetailsConstants.Titles.NOT_FOUND,
                    Status = StatusCodes.Status404NotFound,
                    Detail = result.Error,
                    Instance = $"/api/v1/customers/{id}"
                });
            }

            if (result.Error!.Contains(ApiMessage.Errors.Customer.CUSTOMER_HAS_ASSOCIATED_LASTS))
            {
                return Conflict(new ProblemDetails
                {
                    Type = ProblemDetailsConstants.Types.CONFLICT,
                    Title = ProblemDetailsConstants.Titles.CONFLICT,
                    Status = StatusCodes.Status409Conflict,
                    Detail = result.Error,
                    Instance = $"/api/v1/customers/{id}"
                });
            }

            return BadRequest(result.Error);
        }

        return NoContent();
    }
}