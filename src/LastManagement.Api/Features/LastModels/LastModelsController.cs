using LastManagement.Application.Features.LastModels.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LastManagement.Api.Features.LastModels;

[ApiController]
[Route("api/v1")]
public class LastModelsController : ControllerBase
{
    private readonly GetLastModelsQuery _getLastModelsQuery;
    private readonly GetModelsByLastIdQuery _getModelsByLastIdQuery;
    private readonly ILogger<LastModelsController> _logger;

    public LastModelsController(
        GetLastModelsQuery getLastModelsQuery,
        GetModelsByLastIdQuery getModelsByLastIdQuery,
        ILogger<LastModelsController> logger)
    {
        _getLastModelsQuery = getLastModelsQuery;
        _getModelsByLastIdQuery = getModelsByLastIdQuery;
        _logger = logger;
    }

    /// <summary>
    /// GET /api/v1/last-models
    /// List all last models with filtering
    /// Authorization: Public (Guest) or Admin
    /// </summary>
    [HttpGet("last-models")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLastModels([FromQuery] string? status = null, [FromQuery] string? orderBy = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var models = await _getLastModelsQuery.ExecuteAsync(status, orderBy, cancellationToken);

            var response = new
            {
                value = models,
                count = models.Count()
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving last models");
            return StatusCode(500, new
            {
                type = "http://localhost:5000/problems/internal-error",
                title = "Internal Server Error",
                status = 500,
                detail = "An error occurred while retrieving last models",
                instance = HttpContext.Request.Path.ToString(),
                traceId = HttpContext.TraceIdentifier
            });
        }
    }

    /// <summary>
    /// GET /api/v1/last-names/{lastId}/models
    /// Get models for a specific last (sub-resource)
    /// Authorization: Public (Guest) or Admin
    /// </summary>
    [HttpGet("last-names/{lastId}/models")]
    [AllowAnonymous]
    public async Task<IActionResult> GetModelsByLastId([FromRoute] int lastId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (lastId <= 0)
            {
                return BadRequest(new
                {
                    type = "http://localhost:5000/problems/validation-error",
                    title = "Validation Error",
                    status = 400,
                    detail = "Last ID must be a positive integer",
                    instance = HttpContext.Request.Path.ToString(),
                    traceId = HttpContext.TraceIdentifier,
                    errors = new Dictionary<string, string[]>
                    {
                        { "lastId", new[] { "Must be a positive integer" } }
                    }
                });
            }

            var models = await _getModelsByLastIdQuery.ExecuteAsync(lastId, cancellationToken);

            var response = new
            {
                value = models
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving models for last ID {LastId}", lastId);
            return StatusCode(500, new
            {
                type = "http://localhost:5000/problems/internal-error",
                title = "Internal Server Error",
                status = 500,
                detail = $"An error occurred while retrieving models for last ID {lastId}",
                instance = HttpContext.Request.Path.ToString(),
                traceId = HttpContext.TraceIdentifier
            });
        }
    }
}