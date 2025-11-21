using Asp.Versioning;
using LastManagement.Api.Constants;
using LastManagement.Api.Global.Helpers;
using LastManagement.Application.Features.LastNames.Commands;
using LastManagement.Application.Features.LastNames.DTOs;
using LastManagement.Application.Features.LastNames.Interfaces;
using LastManagement.Application.Features.LastNames.Queries;
using LastManagement.Domain.LastNames.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LastManagement.Api.Features.LastNames;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/last-names")]
public class LastNamesController : ControllerBase
{
    private readonly ILogger<LastNamesController> _logger;

    public LastNamesController(ILogger<LastNamesController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// GET /api/v1/last-names - List all last names with filtering
    /// Authorization: Public (Guest)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLastNames(
        [FromQuery] int limit = 20,
        [FromQuery] int? after = null,
        [FromQuery] int? customerId = null,
        [FromQuery] string? status = null,
        [FromServices] GetLastNamesQuery query = null!,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (limit < 1 || limit > 100)
                limit = 20;

            var (items, totalCount, nextId) = await query.ExecuteAsync(limit, after, customerId, status, cancellationToken);

            var response = new
            {
                value = items.Select(ln => new
                {
                    id = ln.Id,
                    lastCode = ln.LastCode,
                    customerId = ln.CustomerId,
                    status = ln.Status,
                    discontinueReason = ln.DiscontinueReason,
                    createdAt = ln.CreatedAt,
                    updatedAt = ln.UpdatedAt,
                    _links = new
                    {
                        self = new { href = $"/api/v1/last-names/{ln.Id}" },
                        customer = new { href = $"/api/v1/customers/{ln.CustomerId}" },
                        models = new { href = $"/api/v1/last-names/{ln.Id}/models" },
                        inventory = new { href = $"/api/v1/last-names/{ln.Id}/inventory" }
                    }
                }),
                _atNextLink = nextId.HasValue ? $"/api/v1/last-names?limit={limit}&after={nextId.Value}" : null,
                _atCount = totalCount
            };

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                type = "http://localhost:5000/problems/validation-error",
                title = "Validation Error",
                status = 400,
                detail = ex.Message,
                instance = HttpContext.Request.Path.ToString(),
                traceId = HttpContext.TraceIdentifier
            });
        }
    }

    /// <summary>
    /// GET /api/v1/last-names/{id} - Get single last name by ID
    /// Authorization: Public (Guest)
    /// </summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLastNameById(int id, [FromServices] ILastNameRepository repository = null!, CancellationToken cancellationToken = default)
    {
        var lastName = await repository.GetByIdAsync(id, cancellationToken);

        if (lastName == null)
        {
            return NotFound(new
            {
                Type = ProblemDetailsConstants.Types.NOT_FOUND,
                title = "Not Found",
                status = 404,
                detail = $"Last name with ID {id} not found",
                instance = HttpContext.Request.Path.ToString(),
                traceId = HttpContext.TraceIdentifier
            });
        }

        var etag = ETagHelper.Generate(lastName.Version);
        Response.Headers["ETag"] = etag;
        Response.Headers["Cache-Control"] = "private, max-age=60";

        return Ok(new
        {
            id = lastName.Id,
            lastCode = lastName.LastCode,
            customerId = lastName.CustomerId,
            status = lastName.Status,
            discontinueReason = lastName.DiscontinueReason,
            createdAt = lastName.CreatedAt,
            updatedAt = lastName.UpdatedAt,
            _links = new
            {
                self = new { href = $"/api/v1/last-names/{lastName.Id}" },
                customer = new { href = $"/api/v1/customers/{lastName.CustomerId}" },
                models = new { href = $"/api/v1/last-names/{lastName.Id}/models" },
                inventory = new { href = $"/api/v1/last-names/{lastName.Id}/inventory" }
            }
        });
    }

    /// <summary>
    /// POST /api/v1/last-names - Create new last name
    /// Authorization: Admin
    /// </summary>
    [HttpPost]
    [Authorize(Roles = AuthorizationConstants.Roles.ADMIN)]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateLastName([FromBody] CreateLastNameRequest request, [FromServices] CreateLastNameCommand command = null!, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                    );

                return BadRequest(new
                {
                    type = "http://localhost:5000/problems/validation-error",
                    title = "Validation Error",
                    status = 400,
                    errors,
                    instance = HttpContext.Request.Path.ToString(),
                    traceId = HttpContext.TraceIdentifier
                });
            }

            var lastName = await command.ExecuteAsync(request, cancellationToken);

            return CreatedAtAction(
                nameof(GetLastNameById),
                new { id = lastName.LastId },
                new
                {
                    id = lastName.LastId,
                    lastCode = lastName.LastCode,
                    customerId = lastName.CustomerId,
                    status = lastName.Status.ToString(),
                    createdAt = lastName.CreatedAt,
                    updatedAt = lastName.UpdatedAt,
                    _links = new
                    {
                        self = new { href = $"/api/v1/last-names/{lastName.LastId}" },
                        customer = new { href = $"/api/v1/customers/{lastName.CustomerId}" }
                    }
                });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
        {
            return Conflict(new
            {
                Type = ProblemDetailsConstants.Types.CONFLICT,
                Title = ProblemDetailsConstants.Titles.CONFLICT,
                status = 409,
                detail = ex.Message,
                instance = HttpContext.Request.Path.ToString(),
                traceId = HttpContext.TraceIdentifier,
                conflictReason = "duplicate-last-code"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating last name");
            return StatusCode(500, new
            {
                type = "http://localhost:5000/problems/internal-error",
                title = "Internal Server Error",
                status = 500,
                detail = "An error occurred while creating the last name",
                instance = HttpContext.Request.Path.ToString(),
                traceId = HttpContext.TraceIdentifier
            });
        }
    }

    /// <summary>
    /// PATCH /api/v1/last-names/{id} - Update last name (partial)
    /// Authorization: Admin
    /// Requires: If-Match header
    /// </summary>
    [HttpPatch("{id:int}")]
    [Authorize(Roles = AuthorizationConstants.Roles.ADMIN)]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
    public async Task<IActionResult> UpdateLastName(int id, [FromBody] UpdateLastNameRequest request, [FromServices] UpdateLastNameCommand command = null!, [FromServices] ILastNameRepository repository = null!, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check If-Match header
            if (!Request.Headers.TryGetValue("If-Match", out var ifMatchHeader) || string.IsNullOrWhiteSpace(ifMatchHeader))
            {
                return StatusCode(428, new
                {
                    type = "http://localhost:5000/problems/precondition-required",
                    title = "Precondition Required",
                    status = 428,
                    detail = "If-Match header is required for updates",
                    instance = HttpContext.Request.Path.ToString(),
                    traceId = HttpContext.TraceIdentifier
                });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                    );

                return BadRequest(new
                {
                    type = "http://localhost:5000/problems/validation-error",
                    title = "Validation Error",
                    status = 400,
                    errors,
                    instance = HttpContext.Request.Path.ToString(),
                    traceId = HttpContext.TraceIdentifier
                });
            }

            // Get tracked entity for update
            var lastName = await repository.GetByIdForUpdateAsync(id, cancellationToken);
            if (lastName == null)
            {
                return NotFound(new
                {
                    Type = ProblemDetailsConstants.Types.NOT_FOUND,
                    title = "Not Found",
                    status = 404,
                    detail = $"Last name with ID {id} not found",
                    instance = HttpContext.Request.Path.ToString(),
                    traceId = HttpContext.TraceIdentifier
                });
            }

            // Validate ETag
            var providedETag = ifMatchHeader.ToString();
            var currentETag = ETagHelper.Generate(lastName.Version);

            if (providedETag != currentETag)
            {
                return StatusCode(412, new
                {
                    type = "http://localhost:5000/problems/precondition-failed",
                    title = "Precondition Failed",
                    status = 412,
                    detail = "ETag mismatch. Resource was modified.",
                    instance = HttpContext.Request.Path.ToString(),
                    traceId = HttpContext.TraceIdentifier,
                    providedETag = providedETag,
                    currentETag = currentETag
                });
            }

            // Apply updates on tracked entity
            if (!string.IsNullOrWhiteSpace(request.LastCode) && request.LastCode != lastName.LastCode)
            {
                if (await repository.ExistsByCodeAsync(request.LastCode, id, cancellationToken))
                    throw new InvalidOperationException($"Last code '{request.LastCode}' already exists");

                lastName.UpdateLastCode(request.LastCode);
            }

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (!Enum.TryParse<LastNameStatus>(request.Status, true, out var newStatus))
                    throw new ArgumentException($"Invalid status value: {request.Status}");

                lastName.UpdateStatus(newStatus, request.DiscontinueReason);
            }

            if (request.CustomerId.HasValue && request.CustomerId.Value != lastName.CustomerId)
            {
                lastName.TransferToCustomer(request.CustomerId.Value);
            }

            // Save changes
            await repository.UpdateAsync(lastName, cancellationToken);

            // Return updated entity with new ETag
            var newETag = ETagHelper.Generate(lastName.Version);
            Response.Headers["ETag"] = newETag;

            return Ok(new
            {
                id = lastName.LastId,
                lastCode = lastName.LastCode,
                customerId = lastName.CustomerId,
                status = lastName.Status.ToString(),
                discontinueReason = lastName.DiscontinueReason,
                updatedAt = lastName.UpdatedAt,
                _links = new
                {
                    self = new { href = $"/api/v1/last-names/{lastName.LastId}" }
                }
            });
        }
        catch (DbUpdateConcurrencyException)
        {
            return StatusCode(412, new
            {
                type = "http://localhost:5000/problems/precondition-failed",
                title = "Precondition Failed",
                status = 412,
                detail = "The resource was modified by another request. Please refresh and retry.",
                instance = HttpContext.Request.Path.ToString(),
                traceId = HttpContext.TraceIdentifier
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
        {
            return Conflict(new
            {
                Type = ProblemDetailsConstants.Types.CONFLICT,
                Title = ProblemDetailsConstants.Titles.CONFLICT,
                status = 409,
                detail = ex.Message,
                instance = HttpContext.Request.Path.ToString(),
                traceId = HttpContext.TraceIdentifier,
                conflictReason = "duplicate-last-code"
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                type = "http://localhost:5000/problems/validation-error",
                title = "Validation Error",
                status = 400,
                detail = ex.Message,
                instance = HttpContext.Request.Path.ToString(),
                traceId = HttpContext.TraceIdentifier
            });
        }
    }

    /// <summary>
    /// DELETE /api/v1/last-names/{id} - Delete last name
    /// Authorization: Admin
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = AuthorizationConstants.Roles.ADMIN)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteLastName(int id, [FromServices] GetLastNameByIdQuery getQuery = null!, [FromServices] ILastNameRepository repository = null!, CancellationToken cancellationToken = default)
    {
        try
        {
            var lastNameDto = await getQuery.ExecuteAsync(id, cancellationToken);

            if (lastNameDto == null)
            {
                return NotFound(new
                {
                    Type = ProblemDetailsConstants.Types.NOT_FOUND,
                    title = "Not Found",
                    status = 404,
                    detail = $"Last name with ID {id} not found",
                    instance = HttpContext.Request.Path.ToString(),
                    traceId = HttpContext.TraceIdentifier
                });
            }

            // Check dependencies
            var hasModels = await repository.HasModelsAsync(id, cancellationToken);
            var hasInventory = await repository.HasInventoryAsync(id, cancellationToken);
            var hasMovements = await repository.HasMovementsAsync(id, cancellationToken);

            if (hasModels || hasInventory || hasMovements)
            {
                return Conflict(new
                {
                    Type = ProblemDetailsConstants.Types.CONFLICT,
                    Title = ProblemDetailsConstants.Titles.CONFLICT,
                    status = 409,
                    detail = "Cannot delete last name because it has associated records",
                    instance = HttpContext.Request.Path.ToString(),
                    traceId = HttpContext.TraceIdentifier,
                    conflictReason = "has-dependencies",
                    relatedResources = new
                    {
                        hasModels,
                        hasInventory,
                        hasMovements
                    },
                    suggestions = new[]
                    {
                        "Delete or reassign all associated records before deleting the last name",
                        "Change last name status to 'Obsolete' instead using PATCH"
                    }
                });
            }

            // Fetch entity for deletion
            var lastName = await repository.GetByIdAsync(id, cancellationToken);
            if (lastName == null)
            {
                return NotFound();
            }

            await repository.DeleteAsync(lastName!, cancellationToken);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting last name {LastId}", id);
            return StatusCode(500, new
            {
                type = "http://localhost:5000/problems/internal-error",
                title = "Internal Server Error",
                status = 500,
                detail = "An error occurred while deleting the last name",
                instance = HttpContext.Request.Path.ToString(),
                traceId = HttpContext.TraceIdentifier
            });
        }
    }

    /// <summary>
    /// PATCH /api/v1/last-names/$batch - Batch update last names
    /// Authorization: Admin
    /// </summary>
    [HttpPatch("$batch")]
    [Authorize(Roles = AuthorizationConstants.Roles.ADMIN)]
    [ProducesResponseType(typeof(LastNamesBatchOperationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(LastNamesBatchOperationResult), StatusCodes.Status207MultiStatus)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BatchUpdateLastNames([FromBody] UpdateLastNameBatchRequest request, [FromServices] UpdateLastNameBatchCommand command = null!, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                    );

                return BadRequest(new
                {
                    type = "http://localhost:5000/problems/validation-error",
                    title = "Validation Error",
                    status = 400,
                    errors,
                    instance = HttpContext.Request.Path.ToString(),
                    traceId = HttpContext.TraceIdentifier
                });
            }

            var result = await command.ExecuteAsync(request, cancellationToken);

            if (result.Failed == 0)
            {
                return Ok(result);
            }

            return StatusCode(207, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing batch update");
            return StatusCode(500, new
            {
                type = "http://localhost:5000/problems/internal-error",
                title = "Internal Server Error",
                status = 500,
                detail = "An error occurred while processing the batch update",
                instance = HttpContext.Request.Path.ToString(),
                traceId = HttpContext.TraceIdentifier
            });
        }
    }

    /// <summary>
    /// GET /api/v1/customers/{customerId}/lasts - Get all lasts for a customer (sub-resource)
    /// Authorization: Public (Guest)
    /// </summary>
    [HttpGet("/api/v{version:apiVersion}/customers/{customerId:int}/lasts")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCustomerLasts(int customerId, [FromQuery] int limit = 20, [FromQuery] int? after = null, [FromQuery] string? status = null, [FromServices] GetLastNamesQuery query = null!, CancellationToken cancellationToken = default)
    {
        try
        {
            if (limit < 1 || limit > 100)
                limit = 20;

            var (items, totalCount, nextId) = await query.ExecuteAsync(limit, after, customerId, status, cancellationToken);

            var response = new
            {
                value = items.Select(ln => new
                {
                    id = ln.Id,
                    lastCode = ln.LastCode,
                    status = ln.Status,
                    createdAt = ln.CreatedAt,
                    _links = new
                    {
                        self = new { href = $"/api/v1/last-names/{ln.Id}" },
                        customer = new { href = $"/api/v1/customers/{ln.CustomerId}" }
                    }
                }),
                _atNextLink = nextId.HasValue ? $"/api/v1/customers/{customerId}/lasts?limit={limit}&after={nextId.Value}" : null,
                _atCount = totalCount
            };

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                type = "http://localhost:5000/problems/validation-error",
                title = "Validation Error",
                status = 400,
                detail = ex.Message,
                instance = HttpContext.Request.Path.ToString(),
                traceId = HttpContext.TraceIdentifier
            });
        }
    }
}