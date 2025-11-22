using LastManagement.Api.Constants;
using LastManagement.Application.Constants;
using LastManagement.Application.Features.LastModels.Queries;
using LastManagement.Utilities.Helpers;
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
                Type = ProblemDetailsConstants.Types.INTERNAL_ERROR,
                Title = ProblemDetailsConstants.Titles.INTERNAL_SERVER_ERROR,
                Status = 500,
                Detail = ProblemDetailsConstants.Details.RETRIEVING_LAST_MODELS_ERROR,
                Instance = HttpContext.Request.Path,
                TraceId = HttpContext.TraceIdentifier
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
                    Type = ProblemDetailsConstants.Types.VALIDATION_ERROR,
                    Title = ProblemDetailsConstants.Titles.VALIDATION_ERROR,
                    Status = 400,
                    Detail = ErrorMessages.LastModel.LAST_ID_MUST_BE_POSITIVE_INTERGER,
                    Instance = HttpContext.Request.Path,
                    TraceId = HttpContext.TraceIdentifier,
                    Errors = new Dictionary<string, string[]>
                    {
                        { "lastId", new[] { ErrorMessages.LastModel.POSITIVE_INTEGER_ERROR } }
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
                Type = ProblemDetailsConstants.Types.INTERNAL_ERROR,
                Title = ProblemDetailsConstants.Titles.INTERNAL_SERVER_ERROR,
                Status = 500,
                Detail = StringFormatter.FormatMessage(ErrorMessages.LastModel.DETAIL_ERROR, lastId),
                Instance = HttpContext.Request.Path,
                TraceId = HttpContext.TraceIdentifier
            });
        }
    }
}