using Asp.Versioning;
using LastManagement.Application.Features.LastSizes.Commands;
using LastManagement.Application.Features.LastSizes.DTOs;
using LastManagement.Application.Features.LastSizes.Queries;
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
    public async Task<IActionResult> GetLastSizes(
        [FromServices] GetLastSizesQuery query,
        [FromQuery] int limit = 20,
        [FromQuery] int? after = null,
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
                if (filterLower.Contains("status eq 'active'") || filterLower.Contains("status eq active"))
                    statusFilter = "Active";
                else if (filterLower.Contains("status eq 'discontinued'") || filterLower.Contains("status eq discontinued"))
                    statusFilter = "Discontinued";
                else if (filterLower.Contains("status eq 'replaced'") || filterLower.Contains("status eq replaced"))
                    statusFilter = "Replaced";
            }

            var (items, totalCount, nextId) = await query.ExecuteAsync(limit, after, statusFilter, cancellationToken);

            var response = new
            {
                value = items,
                count = totalCount,
                nextLink = nextId.HasValue ? $"/api/v1/last-sizes?limit={limit}&after={nextId}" : null
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting last sizes");
            return StatusCode(500, new
            {
                type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                title = "Internal Server Error",
                status = 500,
                detail = "An error occurred while retrieving last sizes",
                instance = HttpContext.Request.Path,
                traceId = HttpContext.TraceIdentifier
            });
        }
    }

    /// <summary>
    /// GET /api/v1/last-sizes/{id}
    /// Get a specific last size by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLastSizeById(
        int id,
        [FromServices] GetLastSizeByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var lastSize = await query.ExecuteAsync(id, cancellationToken);

            if (lastSize == null)
            {
                return NotFound(new
                {
                    type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
                    title = "Not Found",
                    status = 404,
                    detail = $"Last size with ID {id} not found",
                    instance = HttpContext.Request.Path,
                    traceId = HttpContext.TraceIdentifier
                });
            }

            return Ok(lastSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting last size {SizeId}", id);
            return StatusCode(500, new
            {
                type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                title = "Internal Server Error",
                status = 500,
                detail = "An error occurred while retrieving the last size",
                instance = HttpContext.Request.Path,
                traceId = HttpContext.TraceIdentifier
            });
        }
    }

    /// <summary>
    /// POST /api/v1/last-sizes
    /// Create a new last size
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateLastSize(
        [FromBody] CreateLastSizeRequest request,
        [FromServices] CreateLastSizeCommand command,
        CancellationToken cancellationToken = default)
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
                    type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    title = "Bad Request",
                    status = 400,
                    detail = "One or more validation errors occurred",
                    instance = HttpContext.Request.Path,
                    traceId = HttpContext.TraceIdentifier,
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
                    ["self"] = new { href = $"/api/v1/last-sizes/{lastSize.SizeId}" }
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
                type = "https://tools.ietf.org/html/rfc9110#section-15.5.10",
                title = "Conflict",
                status = 409,
                detail = ex.Message,
                instance = HttpContext.Request.Path,
                traceId = HttpContext.TraceIdentifier,
                conflictReason = "duplicate-size-value"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating last size");
            return StatusCode(500, new
            {
                type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                title = "Internal Server Error",
                status = 500,
                detail = "An error occurred while creating the last size",
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
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateLastSize(
        int id,
        [FromBody] UpdateLastSizeRequest request,
        [FromServices] UpdateLastSizeCommand command,
        CancellationToken cancellationToken = default)
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
                    type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    title = "Bad Request",
                    status = 400,
                    detail = "One or more validation errors occurred",
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
                    ["self"] = new { href = $"/api/v1/last-sizes/{lastSize.SizeId}" }
                }
            };

            return Ok(dto);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
                title = "Not Found",
                status = 404,
                detail = ex.Message,
                instance = HttpContext.Request.Path,
                traceId = HttpContext.TraceIdentifier
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                type = "https://tools.ietf.org/html/rfc9110#section-15.5.10",
                title = "Conflict",
                status = 409,
                detail = ex.Message,
                instance = HttpContext.Request.Path,
                traceId = HttpContext.TraceIdentifier
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating last size {SizeId}", id);
            return StatusCode(500, new
            {
                type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                title = "Internal Server Error",
                status = 500,
                detail = "An error occurred while updating the last size",
                instance = HttpContext.Request.Path,
                traceId = HttpContext.TraceIdentifier
            });
        }
    }

    /// <summary>
    /// DELETE /api/v1/last-sizes/{id}
    /// Delete a last size
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteLastSize(
        int id,
        [FromServices] DeleteLastSizeCommand command,
        CancellationToken cancellationToken = default)
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
                type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
                title = "Not Found",
                status = 404,
                detail = ex.Message,
                instance = HttpContext.Request.Path,
                traceId = HttpContext.TraceIdentifier
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("inventory"))
        {
            return Conflict(new
            {
                type = "https://tools.ietf.org/html/rfc9110#section-15.5.10",
                title = "Conflict",
                status = 409,
                detail = ex.Message,
                instance = HttpContext.Request.Path,
                traceId = HttpContext.TraceIdentifier,
                conflictReason = "has-inventory",
                suggestions = new[]
                {
                    "Remove all inventory records using this size before deleting",
                    "Discontinue the size instead using PATCH with status: Discontinued"
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting last size {SizeId}", id);
            return StatusCode(500, new
            {
                type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                title = "Internal Server Error",
                status = 500,
                detail = "An error occurred while deleting the last size",
                instance = HttpContext.Request.Path,
                traceId = HttpContext.TraceIdentifier
            });
        }
    }
}