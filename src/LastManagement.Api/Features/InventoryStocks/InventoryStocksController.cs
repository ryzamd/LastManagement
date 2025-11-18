using Asp.Versioning;
using LastManagement.Application.Features.InventoryStocks.Commands;
using LastManagement.Application.Features.InventoryStocks.DTOs;
using LastManagement.Application.Features.InventoryStocks.Queries;
using LastManagement.Application.Features.LastSizes.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LastManagement.Api.Features.InventoryStocks;

[ApiController]
[Route("api/v1/inventory")]
[ApiVersion("1.0")]
public class InventoryStocksController : ControllerBase
{
    private readonly GetInventoryStocksQuery _getStocksQuery;
    private readonly GetInventoryStockByIdQuery _getStockByIdQuery;
    private readonly AdjustStockCommand _adjustCommand;
    private readonly TransferStockCommand _transferCommand;
    private readonly BatchAdjustStockCommand _batchAdjustCommand;

    public InventoryStocksController(
        GetInventoryStocksQuery getStocksQuery,
        GetInventoryStockByIdQuery getStockByIdQuery,
        AdjustStockCommand adjustCommand,
        TransferStockCommand transferCommand,
        BatchAdjustStockCommand batchAdjustCommand)
    {
        _getStocksQuery = getStocksQuery;
        _getStockByIdQuery = getStockByIdQuery;
        _adjustCommand = adjustCommand;
        _transferCommand = transferCommand;
        _batchAdjustCommand = batchAdjustCommand;
    }

    /// <summary>
    /// GET /api/v1/inventory/stocks - List inventory stocks
    /// Authorization: Public (Guest)
    /// </summary>
    [HttpGet("stocks")]
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
            nextLink = !string.IsNullOrEmpty(nextCursor)
                ? $"/api/v1/inventory/stocks?limit={limit}&after={nextCursor}"
                : null,
            count = totalCount
        };

        Response.Headers.Append("X-Total-Count", totalCount.ToString());
        Response.Headers.Append("Cache-Control", "private, max-age=30");

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
                type = "http://localhost:5000/problems/not-found",
                title = "Not Found",
                status = 404,
                detail = $"Stock with ID {id} not found",
                instance = $"/api/v1/inventory/stocks/{id}",
                traceId = HttpContext.TraceIdentifier
            });
        }

        var etag = Convert.ToBase64String(
            System.Text.Encoding.UTF8.GetBytes($"{stock.Version}"));
        Response.Headers.Append("ETag", etag);

        return Ok(stock);
    }

    /// <summary>
    /// POST /api/v1/inventory/stocks/{id}/adjust - Adjust stock quantity
    /// Authorization: Admin
    /// </summary>
    [HttpPost("stocks/{id:int}/adjust")]
    [Authorize(Roles = "Admin")]
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
                type = "http://localhost:5000/problems/validation-error",
                title = "Validation Error",
                status = 400,
                errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)),
                instance = $"/api/v1/inventory/stocks/{id}/adjust",
                traceId = HttpContext.TraceIdentifier
            });
        }

        // Validate If-Match header
        if (!Request.Headers.TryGetValue("If-Match", out var ifMatch))
        {
            return BadRequest(new
            {
                type = "http://localhost:5000/problems/validation-error",
                title = "Validation Error",
                status = 400,
                detail = "If-Match header is required for stock adjustments",
                instance = $"/api/v1/inventory/stocks/{id}/adjust",
                traceId = HttpContext.TraceIdentifier
            });
        }

        try
        {
            var adminUser = User.Identity?.Name ?? "system";
            var result = await _adjustCommand.ExecuteAsync(id, request, adminUser, cancellationToken);

            var newEtag = Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes($"{result.Version}"));
            Response.Headers.Append("ETag", $"\"{newEtag}\"");

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                type = "http://localhost:5000/problems/not-found",
                title = "Not Found",
                status = 404,
                detail = ex.Message,
                instance = $"/api/v1/inventory/stocks/{id}/adjust",
                traceId = HttpContext.TraceIdentifier
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                type = "http://localhost:5000/problems/insufficient-stock",
                title = "Insufficient Stock",
                status = 400,
                detail = ex.Message,
                instance = $"/api/v1/inventory/stocks/{id}/adjust",
                traceId = HttpContext.TraceIdentifier
            });
        }
        catch (DbUpdateConcurrencyException)
        {
            return StatusCode(412, new
            {
                type = "http://localhost:5000/problems/precondition-failed",
                title = "Precondition Failed",
                status = 412,
                detail = "Stock was modified by another user",
                instance = $"/api/v1/inventory/stocks/{id}/adjust",
                traceId = HttpContext.TraceIdentifier,
                providedETag = ifMatch.ToString()
            });
        }
    }

    /// <summary>
    /// POST /api/v1/inventory/transfers - Transfer stock between locations
    /// Authorization: Admin
    /// </summary>
    [HttpPost("transfers")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(TransferStockResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TransferStock([FromBody] TransferStockRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                type = "http://localhost:5000/problems/validation-error",
                title = "Validation Error",
                status = 400,
                errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)),
                instance = "/api/v1/inventory/transfers",
                traceId = HttpContext.TraceIdentifier
            });
        }

        try
        {
            var adminUser = User.Identity?.Name ?? "system";
            var result = await _transferCommand.ExecuteAsync(request, adminUser, cancellationToken);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                type = "http://localhost:5000/problems/not-found",
                title = "Not Found",
                status = 404,
                detail = ex.Message,
                instance = "/api/v1/inventory/transfers",
                traceId = HttpContext.TraceIdentifier
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                type = "http://localhost:5000/problems/validation-error",
                title = "Validation Error",
                status = 400,
                detail = ex.Message,
                instance = "/api/v1/inventory/transfers",
                traceId = HttpContext.TraceIdentifier
            });
        }
    }

    /// <summary>
    /// POST /api/v1/inventory/adjustments/$batch - Batch adjust multiple stocks
    /// Authorization: Admin
    /// </summary>
    [HttpPost("adjustments/$batch")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(LastSizesBatchOperationResult), StatusCodes.Status207MultiStatus)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BatchAdjustStocks([FromBody] BatchAdjustmentRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                type = "http://localhost:5000/problems/validation-error",
                title = "Validation Error",
                status = 400,
                errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)),
                instance = "/api/v1/inventory/adjustments/$batch",
                traceId = HttpContext.TraceIdentifier
            });
        }

        var adminUser = User.Identity?.Name ?? "system";
        var result = await _batchAdjustCommand.ExecuteAsync(request, adminUser, cancellationToken);

        return StatusCode(207, result);
    }
}