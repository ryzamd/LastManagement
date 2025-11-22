using Asp.Versioning;
using LastManagement.Api.Constants;
using LastManagement.Api.Global.Helpers;
using LastManagement.Application.Constants;
using LastManagement.Application.Features.LastSizes.Commands;
using LastManagement.Application.Features.LastSizes.DTOs;
using LastManagement.Application.Features.LastSizes.Queries;
using LastManagement.Utilities.Constants.Global;
using LastManagement.Utilities.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LastManagement.Api.Features.LastSizes;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/last-sizes")]
public class LastSizesController : ControllerBase
{
    private readonly ILogger<LastSizesController> _logger;

    public LastSizesController(ILogger<LastSizesController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// GET /api/v1/last-sizes
    /// List all last sizes with filtering and pagination
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetLastSizes([FromServices] GetLastSizesQuery query, [FromQuery] int limit = 20, [FromQuery] int? after = null,
        [FromQuery(Name = "$filter")] string? filter = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Parse filter for status (simple implementation)
            string? statusFilter = null;
            if (!string.IsNullOrWhiteSpace(filter))
            {
                // Support: $filter=status eq 'Active'
                var filterLower = filter.ToLower();

                if (filterLower.Contains(RegexPattern.LastSize.STATUS_ACTIVE_FILTER))
                    statusFilter = StatusConstants.LastSize.ACTIVE;

                else if (filterLower.Contains(RegexPattern.LastSize.STATUS_DISCONTINUED_FILTER))
                    statusFilter = StatusConstants.LastSize.DISCONTINUED;

                else if (filterLower.Contains(RegexPattern.LastSize.STATUS_REPLACED_FILTER))
                    statusFilter = StatusConstants.LastSize.REPLACED;
            }

            var (items, totalCount, nextId) = await query.ExecuteAsync(limit, after, statusFilter, cancellationToken);

            var response = new
            {
                value = items,
                count = totalCount,
                nextLink = nextId.HasValue ? UrlHelper.FormatNextLink(ApiRoutes.LastSizes.FULL_PAGINATION_TEMPLATE, limit, nextId.Value) : null
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting last sizes");
            return StatusCode(500, new
            {
                Type = ProblemDetailsConstants.Types.RFC_INTERNAL_SERVER_ERROR,
                Title = ProblemDetailsConstants.Titles.INTERNAL_SERVER_ERROR,
                Status = 500,
                Detail = ErrorMessages.LastSize.RETREVING_LAST_SIZE_ERROR,
                Instance = HttpContext.Request.Path,
                TraceId = HttpContext.TraceIdentifier
            });
        }
    }

    /// <summary>
    /// GET /api/v1/last-sizes/{id}
    /// Get a specific last size by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLastSizeById(int id, [FromServices] GetLastSizeByIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var lastSize = await query.ExecuteAsync(id, cancellationToken);

            if (lastSize == null)
            {
                return NotFound(new
                {
                    Type = ProblemDetailsConstants.Types.RFC_NOT_FOUND,
                    Title = ProblemDetailsConstants.Titles.NOT_FOUND,
                    Status = 404,
                    Detail = StringFormatter.FormatMessage(ErrorMessages.LastSize.NOT_FOUND, id),
                    Instance = HttpContext.Request.Path,
                    TraceId = HttpContext.TraceIdentifier
                });
            }

            return Ok(lastSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting last size {SizeId}", id);
            return StatusCode(500, new
            {
                Type = ProblemDetailsConstants.Types.RFC_BAD_REQUEST,
                Title = ProblemDetailsConstants.Titles.BAD_REQUEST,
                Detail = ProblemDetailsConstants.Details.INVALID_FIELD_VALUES,
                Instance = HttpContext.Request.Path,
                TraceId = HttpContext.TraceIdentifier
            });
        }
    }

    /// <summary>
    /// POST /api/v1/last-sizes
    /// Create a new last size
    /// </summary>
    [HttpPost]
    [Authorize(Roles = AuthorizationConstants.Roles.ADMIN)]
    public async Task<IActionResult> CreateLastSize([FromBody] CreateLastSizeRequest request, [FromServices] CreateLastSizeCommand command, CancellationToken cancellationToken = default)
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
                    Type = ProblemDetailsConstants.Types.RFC_BAD_REQUEST,
                    Title = ProblemDetailsConstants.Titles.BAD_REQUEST,
                    Status = 400,
                    Detail = ErrorMessages.LastSize.VALIDATION_ERROR,
                    Instance = HttpContext.Request.Path,
                    TraceId = HttpContext.TraceIdentifier,
                    errors
                });
            }

            var lastSize = await command.ExecuteAsync(request, cancellationToken);

            var dto = new LastSizeDto
            {
                Id = lastSize.SizeId,
                SizeValue = lastSize.SizeValue,
                SizeLabel = lastSize.SizeLabel,
                Status = lastSize.Status.ToString(),
                ReplacementSizeId = lastSize.ReplacementSizeId,
                CreatedAt = lastSize.CreatedAt,
                UpdatedAt = lastSize.UpdatedAt,
                Links = new Dictionary<string, object>
                {
                    ["self"] = new { href = UrlHelper.FormatResourceUrl(ApiRoutes.LastSizes.FULL_BY_ID_TEMPLATE, lastSize.SizeId) }
                }
            };

            return CreatedAtAction(
                nameof(GetLastSizeById),
                new { id = lastSize.SizeId },
                dto);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
        {
            return Conflict(new
            {
                type = ProblemDetailsConstants.Types.RFC_CONFLICT,
                Title = ProblemDetailsConstants.Titles.CONFLICT,
                status = 409,
                detail = ex.Message,
                instance = HttpContext.Request.Path,
                traceId = HttpContext.TraceIdentifier,
                conflictReason = ConflictMessages.Reasons.DUPLICATE_SIZE_VALUE,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating last size");
            return StatusCode(500, new
            {
                type = ProblemDetailsConstants.Types.RFC_INTERNAL_SERVER_ERROR,
                title = ProblemDetailsConstants.Titles.INTERNAL_SERVER_ERROR,
                status = 500,
                detail = ErrorMessages.LastSize.CREATE_LAST_SIZE_ERROR,
                instance = HttpContext.Request.Path,
                traceId = HttpContext.TraceIdentifier
            });
        }
    }

    /// <summary>
    /// PATCH /api/v1/last-sizes/{id}
    /// Update a last size
    /// </summary>
    [HttpPatch("{id:int}")]
    [Authorize(Roles = AuthorizationConstants.Roles.ADMIN)]
    public async Task<IActionResult> UpdateLastSize(int id, [FromBody] UpdateLastSizeRequest request, [FromServices] UpdateLastSizeCommand command, CancellationToken cancellationToken = default)
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
                    type = ProblemDetailsConstants.Types.RFC_BAD_REQUEST,
                    title = ProblemDetailsConstants.Titles.BAD_REQUEST,
                    status = 400,
                    detail = ErrorMessages.LastSize.VALIDATION_ERROR,
                    instance = HttpContext.Request.Path,
                    traceId = HttpContext.TraceIdentifier,
                    errors
                });
            }

            var lastSize = await command.ExecuteAsync(id, request, cancellationToken);

            var dto = new LastSizeDto
            {
                Id = lastSize.SizeId,
                SizeValue = lastSize.SizeValue,
                SizeLabel = lastSize.SizeLabel,
                Status = lastSize.Status.ToString(),
                ReplacementSizeId = lastSize.ReplacementSizeId,
                CreatedAt = lastSize.CreatedAt,
                UpdatedAt = lastSize.UpdatedAt,
                Links = new Dictionary<string, object>
                {
                    ["self"] = new { href = UrlHelper.FormatResourceUrl(ApiRoutes.LastSizes.FULL_BY_ID_TEMPLATE, lastSize.SizeId) }
                }
            };

            return Ok(dto);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                Type = ProblemDetailsConstants.Types.RFC_NOT_FOUND,
                Title = ProblemDetailsConstants.Titles.NOT_FOUND,
                Status = 404,
                Detail = ex.Message,
                Instance = HttpContext.Request.Path,
                TraceId = HttpContext.TraceIdentifier
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                Type = ProblemDetailsConstants.Types.RFC_CONFLICT,
                Title = ProblemDetailsConstants.Titles.CONFLICT,
                Status = 409,
                Detail = ex.Message,
                Instance = HttpContext.Request.Path,
                TraceId = HttpContext.TraceIdentifier
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating last size {SizeId}", id);
            return StatusCode(500, new
            {
                Type = ProblemDetailsConstants.Types.RFC_INTERNAL_SERVER_ERROR,
                Title = ProblemDetailsConstants.Titles.INTERNAL_SERVER_ERROR,
                Status = 500,
                Detail = ErrorMessages.LastSize.UPDATE_LAST_SIZE_ERROR,
                Instance = HttpContext.Request.Path,
                TraceId = HttpContext.TraceIdentifier
            });
        }
    }

    /// <summary>
    /// DELETE /api/v1/last-sizes/{id}
    /// Delete a last size
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = AuthorizationConstants.Roles.ADMIN)]
    public async Task<IActionResult> DeleteLastSize(int id, [FromServices] DeleteLastSizeCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            await command.ExecuteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                Type = ProblemDetailsConstants.Types.RFC_NOT_FOUND,
                Title = ProblemDetailsConstants.Titles.NOT_FOUND,
                Status = 404,
                Detail = ex.Message,
                Instance = HttpContext.Request.Path,
                TraceId = HttpContext.TraceIdentifier
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("inventory"))
        {
            return Conflict(new
            {
                Type = ProblemDetailsConstants.Types.RFC_CONFLICT,
                Title = ProblemDetailsConstants.Titles.CONFLICT,
                Status = 409,
                Detail = ex.Message,
                Instance = HttpContext.Request.Path,
                TraceId = HttpContext.TraceIdentifier,
                ConflictReason = ConflictMessages.Reasons.HAS_INVENTORY,
                Suggestions = new[]
                {
                    ConflictMessages.Suggestions.REMOVE_INVENTORY_RECORDS,
                    ConflictMessages.Suggestions.DISCONTINUE_SIZE_INSTEAD
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting last size {SizeId}", id);
            return StatusCode(500, new
            {
                Type = ProblemDetailsConstants.Types.RFC_INTERNAL_SERVER_ERROR,
                Title = ProblemDetailsConstants.Titles.INTERNAL_SERVER_ERROR,
                Status = 500,
                Detail = ErrorMessages.LastSize.DELETE_LAST_SIZE_EROR,
                Instance = HttpContext.Request.Path,
                TraceId = HttpContext.TraceIdentifier
            });
        }
    }

    /// <summary>
    /// POST /api/v1/last-sizes/$batch
    /// Create multiple sizes
    /// </summary>
    [HttpPost("$batch")]
    [Authorize(Roles = AuthorizationConstants.Roles.ADMIN)]
    public async Task<IActionResult> CreateBatch([FromBody] CreateLastSizeBatchRequest request, [FromServices] CreateLastSizeBatchCommand command, CancellationToken cancellationToken = default)
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
                    Type = ProblemDetailsConstants.Types.RFC_BAD_REQUEST,
                    Title = ProblemDetailsConstants.Titles.BAD_REQUEST,
                    Status = 400,
                    Detail = ErrorMessages.LastSize.VALIDATION_ERROR,
                    Instance = HttpContext.Request.Path,
                    TraceId = HttpContext.TraceIdentifier,
                    errors
                });
            }

            var result = await command.ExecuteAsync(request, cancellationToken);

            return StatusCode(207, result); // 207 Multi-Status
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating batch sizes");
            return StatusCode(500, new
            {
                Type = ProblemDetailsConstants.Types.RFC_INTERNAL_SERVER_ERROR,
                Title = ProblemDetailsConstants.Titles.INTERNAL_SERVER_ERROR,
                Status = 500,
                Detail = ErrorMessages.LastSize.CREATE_BATCH_LAST_SIZE_ERROR,
                Instance = HttpContext.Request.Path,
                TraceId = HttpContext.TraceIdentifier
            });
        }
    }

    /// <summary>
    /// PATCH /api/v1/last-sizes/$batch
    /// Update multiple sizes
    /// </summary>
    [HttpPatch("$batch")]
    [Authorize(Roles = AuthorizationConstants.Roles.ADMIN)]
    public async Task<IActionResult> UpdateBatch([FromBody] UpdateLastSizeBatchRequest request, [FromServices] UpdateLastSizeBatchCommand command, CancellationToken cancellationToken = default)
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
                    Type = ProblemDetailsConstants.Types.RFC_BAD_REQUEST,
                    Title = ProblemDetailsConstants.Titles.BAD_REQUEST,
                    Status = 400,
                    Detail = ErrorMessages.LastSize.VALIDATION_ERROR,
                    Instance = HttpContext.Request.Path,
                    TraceId = HttpContext.TraceIdentifier,
                    errors
                });
            }

            var result = await command.ExecuteAsync(request, cancellationToken);

            return StatusCode(207, result); // 207 Multi-Status
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating batch sizes");
            return StatusCode(500, new
            {
                Type = ProblemDetailsConstants.Types.RFC_INTERNAL_SERVER_ERROR,
                Title = ProblemDetailsConstants.Titles.INTERNAL_SERVER_ERROR,
                Status = 500,
                Detail = ErrorMessages.LastSize.UPDATE_BATCH_LAST_SIZE_ERROR,
                Instance = HttpContext.Request.Path,
                TraceId = HttpContext.TraceIdentifier
            });
        }
    }

    /// <summary>
    /// DELETE /api/v1/last-sizes/$batch
    /// Delete multiple sizes
    /// </summary>
    [HttpDelete("$batch")]
    [Authorize(Roles = AuthorizationConstants.Roles.ADMIN)]
    public async Task<IActionResult> DeleteBatch([FromBody] DeleteLastSizeBatchRequest request, [FromServices] DeleteLastSizeBatchCommand command, CancellationToken cancellationToken = default)
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
                    Type = ProblemDetailsConstants.Types.RFC_BAD_REQUEST,
                    Title = ProblemDetailsConstants.Titles.BAD_REQUEST,
                    Status = 400,
                    Detail = ErrorMessages.LastSize.VALIDATION_ERROR,
                    Instance = HttpContext.Request.Path,
                    TraceId = HttpContext.TraceIdentifier,
                    errors
                });
            }

            var result = await command.ExecuteAsync(request, cancellationToken);

            return StatusCode(207, result); // 207 Multi-Status
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting batch sizes");
            return StatusCode(500, new
            {
                Type = ProblemDetailsConstants.Types.RFC_INTERNAL_SERVER_ERROR,
                Title = ProblemDetailsConstants.Titles.INTERNAL_SERVER_ERROR,
                Status = 500,
                Detail = ErrorMessages.LastSize.DELETE_BATCH_LAST_SIZE_ERROR,
                Instance = HttpContext.Request.Path,
                TraceId = HttpContext.TraceIdentifier
            });
        }
    }
}