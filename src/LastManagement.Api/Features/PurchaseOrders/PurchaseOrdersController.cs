using Asp.Versioning;
using LastManagement.Application.Common.Interfaces;
using LastManagement.Application.Features.PurchaseOrders.Commands;
using LastManagement.Application.Features.PurchaseOrders.DTOs;
using LastManagement.Application.Features.PurchaseOrders.Interfaces;
using LastManagement.Application.Features.PurchaseOrders.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace LastManagement.Api.Features.PurchaseOrders;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/purchase-orders")]
public class PurchaseOrdersController : ControllerBase
{
    private readonly ILogger<PurchaseOrdersController> _logger;
    private readonly IIdempotencyService _idempotencyService;
    private readonly ICurrentUserService _currentUserService;

    public PurchaseOrdersController(
        ILogger<PurchaseOrdersController> logger,
        IIdempotencyService idempotencyService,
        ICurrentUserService currentUserService)
    {
        _logger = logger;
        _idempotencyService = idempotencyService;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// GET /api/v1/purchase-orders - List purchase orders with filtering
    /// Authorization: Guest sees own orders, Admin sees all
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPurchaseOrders(
        [FromQuery] int limit = 20,
        [FromQuery] int? after = null,
        [FromQuery] string? status = null,
        [FromQuery] string? requestedBy = null,
        [FromServices] GetPurchaseOrdersQuery query = null!,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (limit < 1 || limit > 100)
                limit = 20;

            // Guest users can only see their own orders
            var isAdmin = User.IsInRole("Admin");
            if (!isAdmin && !string.IsNullOrWhiteSpace(_currentUserService.Username))
            {
                requestedBy = _currentUserService.Username;
            }

            var (orders, totalCount, nextId) = await query.ExecuteAsync(
                limit,
                after,
                status,
                requestedBy,
                cancellationToken);

            var response = new
            {
                value = orders.Select(po => new
                {
                    id = po.Id,
                    orderNumber = po.OrderNumber,
                    locationId = po.LocationId,
                    locationName = po.LocationName,
                    status = po.Status,
                    requestedBy = po.RequestedBy,
                    department = po.Department,
                    itemCount = po.ItemCount,
                    totalQuantity = po.TotalQuantity,
                    createdAt = po.CreatedAt,
                    reviewedAt = po.ReviewedAt,
                    reviewedBy = po.ReviewedBy,
                    _links = new
                    {
                        self = new { href = $"/api/v1/purchase-orders/{po.Id}" },
                        items = new { href = $"/api/v1/purchase-orders/{po.Id}?$expand=items" },
                        confirm = po.Status == "Pending" && isAdmin ? new
                        {
                            href = $"/api/v1/purchase-orders/{po.Id}/confirm",
                            method = "POST",
                            requires = new[] { "Admin" }
                        } : null,
                        deny = po.Status == "Pending" && isAdmin ? new
                        {
                            href = $"/api/v1/purchase-orders/{po.Id}/deny",
                            method = "POST",
                            requires = new[] { "Admin" }
                        } : null
                    }
                }),
                _atNextLink = nextId.HasValue ? $"/api/v1/purchase-orders?limit={limit}&after={nextId.Value}" : null,
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
    /// GET /api/v1/purchase-orders/{id} - Get single purchase order
    /// Authorization: Public (Guest)
    /// </summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPurchaseOrderById(int id, [FromQuery(Name = "$expand")] string? expand, [FromServices] GetPurchaseOrderByIdQuery query = null!, CancellationToken cancellationToken = default)
    {
        var expandItems = expand?.Contains("items", StringComparison.OrdinalIgnoreCase) ?? false;

        var order = await query.ExecuteAsync(id, expandItems, cancellationToken);

        if (order == null)
        {
            return NotFound(new
            {
                type = "http://localhost:5000/problems/not-found",
                title = "Not Found",
                status = 404,
                detail = $"Purchase order with ID {id} not found",
                instance = HttpContext.Request.Path.ToString(),
                traceId = HttpContext.TraceIdentifier
            });
        }

        var response = new
        {
            id = order.Id,
            orderNumber = order.OrderNumber,
            locationId = order.LocationId,
            locationName = order.LocationName,
            status = order.Status,
            requestedBy = order.RequestedBy,
            department = order.Department,
            notes = order.Notes,
            adminNotes = order.AdminNotes,
            createdAt = order.CreatedAt,
            reviewedAt = order.ReviewedAt,
            reviewedBy = order.ReviewedBy,
            items = expandItems ? order.Items?.Select(item => new
            {
                id = item.Id,
                lastId = item.LastId,
                lastCode = item.LastCode,
                customerName = item.CustomerName,
                sizeId = item.SizeId,
                sizeLabel = item.SizeLabel,
                quantityRequested = item.QuantityRequested,
                _links = new
                {
                    last = new { href = $"/api/v1/last-names/{item.LastId}" },
                    size = new { href = $"/api/v1/last-sizes/{item.SizeId}" }
                }
            }) : null,
            summary = new
            {
                totalItems = order.ItemCount,
                totalQuantity = order.TotalQuantity
            },
            _links = new
            {
                self = new { href = $"/api/v1/purchase-orders/{order.Id}" },
                location = new { href = $"/api/v1/locations/{order.LocationId}" },
                inventory = new { href = $"/api/v1/inventory/stocks?$filter=locationId eq {order.LocationId}" }
            }
        };

        return Ok(response);
    }

    /// <summary>
    /// POST /api/v1/purchase-orders - Create purchase order
    /// Authorization: Public (Guest can create)
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreatePurchaseOrder(
        [FromBody] CreatePurchaseOrderRequest request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        [FromServices] CreatePurchaseOrderCommand command = null!,
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
                    type = "http://localhost:5000/problems/validation-error",
                    title = "Validation Error",
                    status = 400,
                    errors,
                    instance = HttpContext.Request.Path.ToString(),
                    traceId = HttpContext.TraceIdentifier
                });
            }

            // Check idempotency key
            if (!string.IsNullOrWhiteSpace(idempotencyKey))
            {
                var existingResult = await _idempotencyService.CheckKeyAsync(idempotencyKey, cancellationToken);
                if (existingResult != null)
                {
                    var cachedResponse = JsonSerializer.Deserialize<object>(existingResult);
                    return Ok(cachedResponse);
                }
            }

            var order = await command.ExecuteAsync(request, cancellationToken);

            var response = new
            {
                id = order.OrderId,
                orderNumber = order.OrderNumber,
                locationId = order.LocationId,
                status = order.Status.ToString(),
                requestedBy = order.RequestedBy,
                department = order.Department,
                notes = order.Notes,
                createdAt = order.CreatedAt,
                items = request.Items.Select(item => new
                {
                    lastId = item.LastId,
                    sizeId = item.SizeId,
                    quantityRequested = item.QuantityRequested
                }),
                _links = new
                {
                    self = new { href = $"/api/v1/purchase-orders/{order.OrderId}" }
                }
            };

            // Store idempotency result
            if (!string.IsNullOrWhiteSpace(idempotencyKey))
            {
                var resultJson = JsonSerializer.Serialize(response);
                await _idempotencyService.StoreResultAsync(
                    idempotencyKey,
                    resultJson,
                    DateTime.UtcNow.AddHours(24),
                    cancellationToken);
            }

            Response.Headers["Location"] = $"/api/v1/purchase-orders/{order.OrderId}";
            return CreatedAtAction(
                nameof(GetPurchaseOrderById),
                new { id = order.OrderId },
                response);
        }
        catch (InvalidOperationException ex)
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
    /// POST /api/v1/purchase-orders/{id}/confirm - Confirm purchase order
    /// Authorization: Admin only
    /// </summary>
    [HttpPost("{id:int}/confirm")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ConfirmPurchaseOrder(
        int id,
        [FromBody] ConfirmOrderRequest request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        [FromServices] ConfirmPurchaseOrderCommand command = null!,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Check idempotency key
            if (!string.IsNullOrWhiteSpace(idempotencyKey))
            {
                var existingResult = await _idempotencyService.CheckKeyAsync(idempotencyKey, cancellationToken);
                if (existingResult != null)
                {
                    var cachedResponse = JsonSerializer.Deserialize<object>(existingResult);
                    return Ok(cachedResponse);
                }
            }

            var reviewedBy = _currentUserService.Username ?? "admin";
            var (order, inventoryUpdates) = await command.ExecuteAsync(id, reviewedBy, request, cancellationToken);

            var response = new
            {
                id = order.Id,
                orderNumber = order.OrderNumber,
                status = order.Status,
                reviewedAt = order.ReviewedAt,
                reviewedBy = order.ReviewedBy,
                adminNotes = order.AdminNotes,
                inventoryUpdates = inventoryUpdates.Select(u => new
                {
                    stockId = u.StockId,
                    lastCode = u.LastCode,
                    sizeLabel = u.SizeLabel,
                    locationName = u.LocationName,
                    previousQuantity = u.PreviousQuantity,
                    newQuantity = u.NewQuantity,
                    movementId = u.MovementId,
                    _links = new
                    {
                        stock = new { href = $"/api/v1/inventory/stocks/{u.StockId}" },
                        movement = new { href = $"/api/v1/inventory/movements/{u.MovementId}" }
                    }
                }),
                _links = new
                {
                    self = new { href = $"/api/v1/purchase-orders/{order.Id}" },
                    inventory = new { href = $"/api/v1/locations/{order.LocationId}/inventory" }
                }
            };

            // Store idempotency result
            if (!string.IsNullOrWhiteSpace(idempotencyKey))
            {
                var resultJson = JsonSerializer.Serialize(response);
                await _idempotencyService.StoreResultAsync(
                    idempotencyKey,
                    resultJson,
                    DateTime.UtcNow.AddHours(24),
                    cancellationToken);
            }

            return Ok(response);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return NotFound(new
            {
                type = "http://localhost:5000/problems/not-found",
                title = "Not Found",
                status = 404,
                detail = ex.Message,
                instance = HttpContext.Request.Path.ToString(),
                traceId = HttpContext.TraceIdentifier
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("status"))
        {
            return Conflict(new
            {
                type = "http://localhost:5000/problems/order-already-reviewed",
                title = "Order Already Reviewed",
                status = 409,
                detail = ex.Message,
                instance = HttpContext.Request.Path.ToString(),
                traceId = HttpContext.TraceIdentifier
            });
        }
    }

    /// <summary>
    /// POST /api/v1/purchase-orders/{id}/deny - Deny purchase order
    /// Authorization: Admin only
    /// </summary>
    [HttpPost("{id:int}/deny")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DenyPurchaseOrder(
        int id,
        [FromBody] DenyOrderRequest request,
        [FromServices] DenyPurchaseOrderCommand command = null!,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var reviewedBy = _currentUserService.Username ?? "admin";
            var order = await command.ExecuteAsync(id, reviewedBy, request, cancellationToken);

            var response = new
            {
                id = order.Id,
                orderNumber = order.OrderNumber,
                status = order.Status,
                reviewedAt = order.ReviewedAt,
                reviewedBy = order.ReviewedBy,
                adminNotes = order.AdminNotes,
                _links = new
                {
                    self = new { href = $"/api/v1/purchase-orders/{order.Id}" }
                }
            };

            return Ok(response);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return NotFound(new
            {
                type = "http://localhost:5000/problems/not-found",
                title = "Not Found",
                status = 404,
                detail = ex.Message,
                instance = HttpContext.Request.Path.ToString(),
                traceId = HttpContext.TraceIdentifier
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("status"))
        {
            return Conflict(new
            {
                type = "http://localhost:5000/problems/order-already-reviewed",
                title = "Order Already Reviewed",
                status = 409,
                detail = ex.Message,
                instance = HttpContext.Request.Path.ToString(),
                traceId = HttpContext.TraceIdentifier
            });
        }
    }

    /// <summary>
    /// GET /api/v1/purchase-orders/pending - Get pending orders (Admin dashboard)
    /// Authorization: Admin only
    /// </summary>
    [HttpGet("pending")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendingOrders(
        [FromServices] GetPendingOrdersQuery query = null!,
        CancellationToken cancellationToken = default)
    {
        var (orders, summary) = await query.ExecuteAsync(cancellationToken);

        var response = new
        {
            value = orders.Select(po => new
            {
                id = po.Id,
                orderNumber = po.OrderNumber,
                locationId = po.LocationId,
                locationName = po.LocationName,
                requestedBy = po.RequestedBy,
                department = po.Department,
                itemCount = po.ItemCount,
                totalQuantity = po.TotalQuantity,
                createdAt = po.CreatedAt,
                _links = new
                {
                    self = new { href = $"/api/v1/purchase-orders/{po.Id}" },
                    confirm = new
                    {
                        href = $"/api/v1/purchase-orders/{po.Id}/confirm",
                        method = "POST"
                    },
                    deny = new
                    {
                        href = $"/api/v1/purchase-orders/{po.Id}/deny",
                        method = "POST"
                    }
                }
            }),
            summary = new
            {
                totalPending = summary.TotalPending,
                totalItemsRequested = summary.TotalItemsRequested,
                totalQuantityRequested = summary.TotalQuantityRequested
            }
        };

        return Ok(response);
    }
}