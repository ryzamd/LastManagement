using Asp.Versioning;
using LastManagement.Api.Constants;
using LastManagement.Api.Global.Helpers;
using LastManagement.Application.Constants;
using LastManagement.Application.Features.InventoryStocks.Commands;
using LastManagement.Application.Features.InventoryStocks.DTOs;
using LastManagement.Application.Features.InventoryStocks.Queries;
using LastManagement.Application.Features.LastSizes.DTOs;
using LastManagement.Utilities.Constants.Global;
using LastManagement.Utilities.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace LastManagement.Api.Features.InventoryStocks;

[ApiController]
[Route("api/v1/inventory")]
[ApiVersion("1.0")]
public class InventoryStocksController : ControllerBase
{
    private readonly GetInventoryStocksQuery _getStocksQuery;
    private readonly GetInventoryStockByIdQuery _getStockByIdQuery;
    private readonly GetInventorySummaryQuery _getSummaryQuery;
    private readonly GetLowStockQuery _getLowStockQuery;
    private readonly GetInventoryMovementsQuery _getMovementsQuery;
    private readonly AdjustStockCommand _adjustCommand;
    private readonly TransferStockCommand _transferCommand;
    private readonly BatchAdjustStockCommand _batchAdjustCommand;

    public InventoryStocksController(
        GetInventoryStocksQuery getStocksQuery,
        GetInventoryStockByIdQuery getStockByIdQuery,
        GetInventorySummaryQuery getSummaryQuery,
        GetLowStockQuery getLowStockQuery,
        GetInventoryMovementsQuery getMovementsQuery,
        AdjustStockCommand adjustCommand,
        TransferStockCommand transferCommand,
        BatchAdjustStockCommand batchAdjustCommand)
    {
        _getStocksQuery = getStocksQuery;
        _getStockByIdQuery = getStockByIdQuery;
        _getSummaryQuery = getSummaryQuery;
        _getLowStockQuery = getLowStockQuery;
        _getMovementsQuery = getMovementsQuery;
        _adjustCommand = adjustCommand;
        _transferCommand = transferCommand;
        _batchAdjustCommand = batchAdjustCommand;
    }

    /// <summary>
    /// GET /api/v1/inventory/stocks - List inventory stocks
    /// Authorization: Public (Guest)
    /// </summary>
    [HttpGet(ApiRoutes.Inventory.STOCKS)]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStocks(
        [FromQuery] int? lastId,
        [FromQuery] int? sizeId,
        [FromQuery] int? locationId,
        [FromQuery] string? after,
        [FromQuery] int limit = 20,
        CancellationToken cancellationToken = default)
    {
        if (limit > 100) limit = 100;

        var (items, totalCount, nextCursor) = await _getStocksQuery.ExecuteAsync(
            lastId, sizeId, locationId, after, limit, cancellationToken);

        var response = new
        {
            value = items,
            nextLink = !string.IsNullOrEmpty(nextCursor) ? UrlHelper.FormatResourceUrl(ApiRoutes.Inventory.FULL_STOCKS_PAGINATION, limit, nextCursor) : null,
            count = totalCount
        };

        Response.Headers.Append(HttpConstants.Headers.X_TOTAL_COUNT, totalCount.ToString());
        Response.Headers.Append(HttpConstants.Headers.CACHE_CONTROL, CacheConstants.CacheControl.PRIVATE_MAX_AGE_30);

        return Ok(response);
    }

    /// <summary>
    /// GET /api/v1/inventory/stocks/{id} - Get single stock
    /// Authorization: Public (Guest)
    /// </summary>
    [HttpGet("stocks/{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(InventoryStockDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStock(int id, CancellationToken cancellationToken)
    {
        var stock = await _getStockByIdQuery.ExecuteAsync(id, cancellationToken);
        if (stock == null)
        {
            return NotFound(new
            {
                Type = ProblemDetailsConstants.Types.NOT_FOUND,
                Title = ProblemDetailsConstants.Titles.NOT_FOUND,
                Status = 404,
                Detail = StringFormatter.FormatMessage(ErrorMessages.Stock.NOT_FOUND, id),
                Instance = UrlHelper.FormatInstancePath(ApiRoutes.Inventory.FULL_BY_ID_TEMPLATE, id),
                TraceId = HttpContext.TraceIdentifier
            });
        }

        var etag = Convert.ToBase64String(Encoding.UTF8.GetBytes(stock.Version.ToString()));
        Response.Headers.Append(HttpConstants.Headers.ETAG, etag);

        return Ok(stock);
    }

    /// <summary>
    /// POST /api/v1/inventory/stocks/{id}/adjust - Adjust stock quantity
    /// Authorization: Admin
    /// </summary>
    [HttpPost("stocks/{id:int}/adjust")]
    [Authorize(Roles = AuthorizationConstants.Roles.ADMIN)]
    [ProducesResponseType(typeof(AdjustStockResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
    public async Task<IActionResult> AdjustStock(int id, [FromBody] AdjustStockRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                Type = ProblemDetailsConstants.Types.VALIDATION_ERROR,
                Title = ProblemDetailsConstants.Titles.VALIDATION_ERROR,
                Status = 400,
                Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)),
                Instance = UrlHelper.FormatInstancePath(ApiRoutes.Inventory.FULL_ADJUST_TEMPLATE, id),
                TraceId = HttpContext.TraceIdentifier
            });
        }

        if (!Request.Headers.TryGetValue(HttpConstants.Headers.IF_MATCH, out var ifMatch))
        {
            return BadRequest(new
            {
                Type = ProblemDetailsConstants.Types.VALIDATION_ERROR,
                Title = ProblemDetailsConstants.Titles.VALIDATION_ERROR,
                Status = 400,
                Detail = ErrorMessages.Stock.IF_MATCH_REQUIRED_FOR_ADJUSTMENT,
                Instance = UrlHelper.FormatInstancePath(ApiRoutes.Inventory.FULL_ADJUST_TEMPLATE, id),
                TraceId = HttpContext.TraceIdentifier
            });
        }

        try
        {
            var adminUser = User.Identity?.Name ?? RoleConstants.SYSTEM;
            var result = await _adjustCommand.ExecuteAsync(id, request, adminUser, cancellationToken);

            var newEtag = Convert.ToBase64String(Encoding.UTF8.GetBytes(result.Version.ToString()));
            Response.Headers.Append(HttpConstants.Headers.ETAG, $"\"{newEtag}\"");

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                Type = ProblemDetailsConstants.Types.NOT_FOUND,
                Title = ProblemDetailsConstants.Titles.NOT_FOUND,
                Status = 404,
                Detail = ex.Message,
                Instance = UrlHelper.FormatInstancePath(ApiRoutes.Inventory.FULL_ADJUST_TEMPLATE, id),
                TraceId = HttpContext.TraceIdentifier
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                Type = ProblemDetailsConstants.Types.INSUFFICIENT_STOCK,
                Title = ProblemDetailsConstants.Titles.INSUFFICIENT_STOCK,
                Status = 400,
                Detail = ex.Message,
                Instance = UrlHelper.FormatInstancePath(ApiRoutes.Inventory.FULL_ADJUST_TEMPLATE, id),
                TraceId = HttpContext.TraceIdentifier
            });
        }
        catch (DbUpdateConcurrencyException)
        {
            return StatusCode(412, new
            {
                Type = ProblemDetailsConstants.Types.PRECONDITION_FAILED,
                Title = ProblemDetailsConstants.Titles.PRECONDITION_FAILED,
                Status = 412,
                Detail = ErrorMessages.Stock.STOCK_WAS_MODIFIED_BY_ANOTHER,
                Instance = UrlHelper.FormatInstancePath(ApiRoutes.Inventory.FULL_ADJUST_TEMPLATE, id),
                TraceId = HttpContext.TraceIdentifier,
                ProvidedETag = ifMatch.ToString()
            });
        }
    }

    /// <summary>
    /// POST /api/v1/inventory/transfers - Transfer stock between locations
    /// Authorization: Admin
    /// </summary>
    [HttpPost("transfers")]
    [Authorize(Roles = AuthorizationConstants.Roles.ADMIN)]
    [ProducesResponseType(typeof(TransferStockResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TransferStock([FromBody] TransferStockRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                Type = ProblemDetailsConstants.Types.VALIDATION_ERROR,
                Sitle = ProblemDetailsConstants.Titles.VALIDATION_ERROR,
                Status = 400,
                Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)),
                Instance = ApiRoutes.InventoryStocks.FULL_TRANSFER,
                TraceId = HttpContext.TraceIdentifier
            });
        }

        try
        {
            var adminUser = User.Identity?.Name ?? RoleConstants.SYSTEM;
            var result = await _transferCommand.ExecuteAsync(request, adminUser, cancellationToken);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                Type = ProblemDetailsConstants.Types.NOT_FOUND,
                Title = ProblemDetailsConstants.Titles.NOT_FOUND,
                Status = 404,
                Detail = ex.Message,
                Instance = ApiRoutes.InventoryStocks.FULL_TRANSFER,
                TraceId = HttpContext.TraceIdentifier
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                Type = ProblemDetailsConstants.Types.VALIDATION_ERROR,
                Sitle = ProblemDetailsConstants.Titles.VALIDATION_ERROR,
                Status = 400,
                Detail = ex.Message,
                Instance = ApiRoutes.InventoryStocks.FULL_TRANSFER,
                TraceId = HttpContext.TraceIdentifier
            });
        }
    }

    /// <summary>
    /// POST /api/v1/inventory/adjustments/$batch - Batch adjust multiple stocks
    /// Authorization: Admin
    /// </summary>
    [HttpPost("adjustments/$batch")]
    [Authorize(Roles = AuthorizationConstants.Roles.ADMIN)]
    [ProducesResponseType(typeof(LastSizesBatchOperationResult), StatusCodes.Status207MultiStatus)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BatchAdjustStocks([FromBody] BatchAdjustmentRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                Type = ProblemDetailsConstants.Types.VALIDATION_ERROR,
                Sitle = ProblemDetailsConstants.Titles.VALIDATION_ERROR,
                Status = 400,
                Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)),
                Instance = ApiRoutes.InventoryStocks.FULL_ADJUSTMENT_BATCH,
                TraceId = HttpContext.TraceIdentifier
            });
        }

        var adminUser = User.Identity?.Name ?? RoleConstants.SYSTEM;
        var result = await _batchAdjustCommand.ExecuteAsync(request, adminUser, cancellationToken);

        return StatusCode(207, result);
    }

    /// <summary>
    /// GET /api/v1/inventory/summary - Get aggregated inventory summary
    /// Authorization: Public (Guest)
    /// </summary>
    [HttpGet("summary")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummary(
        [FromQuery] int? customerId,
        [FromQuery] int? lastId,
        [FromQuery] int? locationId,
        CancellationToken cancellationToken = default)
    {
        var items = await _getSummaryQuery.ExecuteAsync(customerId, lastId, locationId, cancellationToken);

        var response = new
        {
            value = items
        };

        return Ok(response);
    }

    /// <summary>
    /// GET /api/v1/inventory/low-stock - Get low stock alerts
    /// Authorization: Public (Guest)
    /// </summary>
    [HttpGet("low-stock")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLowStock([FromQuery] int threshold = 10, CancellationToken cancellationToken = default)
    {
        if (threshold <= 0)
        {
            return BadRequest(new
            {
                Type = ProblemDetailsConstants.Types.VALIDATION_ERROR,
                Sitle = ProblemDetailsConstants.Titles.VALIDATION_ERROR,
                Status = 400,
                Detail = ErrorMessages.Inventory.THRESHOLD_MUST_BE_POSITIVE_INTEGER,
                Instance = ApiRoutes.InventoryStocks.FULL_LOW_STOCK,
                TraceId = HttpContext.TraceIdentifier
            });
        }

        var (items, summary) = await _getLowStockQuery.ExecuteAsync(threshold, cancellationToken);

        var response = new
        {
            value = items,
            count = summary.TotalLowStockItems,
            summary = new
            {
                totalLowStockItems = summary.TotalLowStockItems,
                criticalItems = summary.CriticalItems,
                warningItems = summary.WarningItems
            }
        };

        return Ok(response);
    }

    /// <summary>
    /// GET /api/v1/inventory/movements - Get movement history
    /// Authorization: Public (Guest)
    /// </summary>
    [HttpGet("movements")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMovements(
        [FromQuery] int? lastId,
        [FromQuery] string? movementType,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] string? after,
        [FromQuery] int limit = 20,
        CancellationToken cancellationToken = default)
    {
        if (limit > 100) limit = 100;

        // Enforce UTC timezone
        if (fromDate.HasValue && fromDate.Value.Kind != DateTimeKind.Utc)
            fromDate = DateTime.SpecifyKind(fromDate.Value, DateTimeKind.Utc);
        if (toDate.HasValue && toDate.Value.Kind != DateTimeKind.Utc)
            toDate = DateTime.SpecifyKind(toDate.Value, DateTimeKind.Utc);

        var (items, totalCount, nextCursor) = await _getMovementsQuery.ExecuteAsync(
            lastId, movementType, fromDate, toDate, after, limit, cancellationToken);

        var response = new
        {
            value = items,
            nextLink = !string.IsNullOrEmpty(nextCursor) ? UrlHelper.FormatResourceUrl(ApiRoutes.Inventory.FULL_MOVEMENTS_PAGINATION, limit, nextCursor) : null,
            count = totalCount
        };

        return Ok(response);
    }
}