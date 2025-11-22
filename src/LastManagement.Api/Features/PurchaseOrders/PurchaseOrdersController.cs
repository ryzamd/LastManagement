using Asp.Versioning;
using LastManagement.Api.Constants;
using LastManagement.Api.Global.Helpers;
using LastManagement.Application.Common.Interfaces;
using LastManagement.Application.Constants;
using LastManagement.Application.Features.PurchaseOrders.Commands;
using LastManagement.Application.Features.PurchaseOrders.DTOs;
using LastManagement.Application.Features.PurchaseOrders.Interfaces;
using LastManagement.Application.Features.PurchaseOrders.Queries;
using LastManagement.Domain.PurchaseOrders.Enums;
using LastManagement.Utilities.Constants.Global;
using LastManagement.Utilities.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace LastManagement.Api.Features.PurchaseOrders;

[ApiController]
[ApiVersion("1.0")]
[Route(ApiRoutes.PurchaseOrders.BASE)]
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
            var isAdmin = User.IsInRole(AuthorizationConstants.Roles.ADMIN);
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
                        self = new { href = UrlHelper.FormatInstancePath(ApiRoutes.PurchaseOrders.FULL_BY_ID_TEMPLATE, po.Id) },
                        items = new { href = UrlHelper.FormatInstancePath(ApiRoutes.PurchaseOrders.FULL_WITH_ITEMS_TEMPLATE, po.Id) },
                        confirm = po.Status == nameof(PurchaseOrderStatus.Pending) && isAdmin ? new
                        {
                            href = UrlHelper.FormatInstancePath(ApiRoutes.PurchaseOrders.FULL_CONFIRM_TEMPLATE, po.Id),
                            method = HttpConstants.Methods.POST,
                            requires = new[] { AuthorizationConstants.Roles.ADMIN }
                        } : null,
                        deny = po.Status == nameof(PurchaseOrderStatus.Pending) && isAdmin ? new
                        {
                            href = UrlHelper.FormatInstancePath(ApiRoutes.PurchaseOrders.FULL_DENY_TEMPLATE, po.Id),
                            method = HttpConstants.Methods.POST,
                            requires = new[] { AuthorizationConstants.Roles.ADMIN }
                        } : null
                    }
                }),
                _atNextLink = nextId.HasValue ? UrlHelper.FormatNextLink(ApiRoutes.PurchaseOrders.FULL_PAGINATION_TEMPLATE, limit, nextId.Value) : null,
                _atCount = totalCount
            };

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                type = ProblemDetailsConstants.Types.VALIDATION_ERROR,
                title = ProblemDetailsConstants.Titles.VALIDATION_ERROR,
                status = 400,
                detail = ex.Message,
                instance = HttpContext.Request.Path,
                traceId = HttpContext.TraceIdentifier
            });
        }
    }

    /// <summary>
    /// GET /api/v1/purchase-orders/{id} - Get single purchase order
    /// Authorization: Public (Guest)
    /// </summary>
    [HttpGet(ApiRoutes.PurchaseOrders.BY_ID)]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPurchaseOrderById(int id, [FromQuery(Name = ApiRoutes.QueryParameters.EXPAND)] string? expand, [FromServices] GetPurchaseOrderByIdQuery query = null!, CancellationToken cancellationToken = default)
    {
        var expandItems = expand?.Contains(ApiRoutes.QueryParameters.EXPAND_ITEMS, StringComparison.OrdinalIgnoreCase) ?? false;

        var order = await query.ExecuteAsync(id, expandItems, cancellationToken);

        if (order == null)
        {
            return NotFound(new
            {
                Type = ProblemDetailsConstants.Types.NOT_FOUND_ERROR,
                Title = ProblemDetailsConstants.Titles.NOT_FOUND,
                Status = 404,
                Detail = StringFormatter.FormatMessage(ErrorMessages.PurchaseOrder.NOT_FOUND, id),
                Instance = HttpContext.Request.Path,
                TraceId = HttpContext.TraceIdentifier
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
                    last = new { href = UrlHelper.FormatInstancePath(ApiRoutes.LastNames.FULL_BY_ID_TEMPLATE, item.LastId) },
                    size = new { href = UrlHelper.FormatInstancePath(ApiRoutes.LastSizes.FULL_BY_ID_TEMPLATE, item.SizeId) }
                }
            }) : null,
            summary = new
            {
                totalItems = order.ItemCount,
                totalQuantity = order.TotalQuantity
            },
            _links = new
            {
                self = new { href = UrlHelper.FormatInstancePath(ApiRoutes.PurchaseOrders.FULL_BY_ID_TEMPLATE, order.Id) },
                location = new { href = UrlHelper.FormatInstancePath(ApiRoutes.Locations.FULL_BY_ID_TEMPLATE, order.LocationId) },
                inventory = new { href = UrlHelper.FormatInstancePath(ApiRoutes.Inventory.FULL_STOCKS_FILTER_TEMPLATE, order.LocationId) }
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
        [FromHeader(Name = HttpConstants.Headers.IDEMPOTENCY_KEY)] string? idempotencyKey,
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
                    Type = ProblemDetailsConstants.Types.VALIDATION_ERROR,
                    Sitle = ProblemDetailsConstants.Titles.VALIDATION_ERROR,
                    Status = 400,
                    Detail = errors,
                    Instance = HttpContext.Request.Path,
                    TraceId = HttpContext.TraceIdentifier
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
                    self = new { href = UrlHelper.FormatInstancePath(ApiRoutes.PurchaseOrders.FULL_BY_ID_TEMPLATE, order.OrderId) }
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

            Response.Headers.Location = UrlHelper.FormatInstancePath(ApiRoutes.PurchaseOrders.FULL_BY_ID_TEMPLATE, order.OrderId);
            return CreatedAtAction(
                nameof(GetPurchaseOrderById),
                new { id = order.OrderId },
                response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                Type = ProblemDetailsConstants.Types.VALIDATION_ERROR,
                Title = ProblemDetailsConstants.Titles.VALIDATION_ERROR,
                Status = 400,
                Detail = ex.Message,
                Instance = HttpContext.Request.Path,
                TraceId = HttpContext.TraceIdentifier
            });
        }
    }

    /// <summary>
    /// POST /api/v1/purchase-orders/{id}/confirm - Confirm purchase order
    /// Authorization: Admin only
    /// </summary>
    [HttpPost(ApiRoutes.PurchaseOrders.CONFIRM)]
    [Authorize(Roles = AuthorizationConstants.Roles.ADMIN)]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ConfirmPurchaseOrder(
        int id,
        [FromBody] ConfirmOrderRequest request,
        [FromHeader(Name = HttpConstants.Headers.IDEMPOTENCY_KEY)] string? idempotencyKey,
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

            var reviewedBy = _currentUserService.Username ?? RoleConstants.ADMIN;
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
                        stock = new { href = UrlHelper.FormatInstancePath(ApiRoutes.PurchaseOrders.FULL_BY_ID_TEMPLATE, u.StockId) },
                        movement = new { href = UrlHelper.FormatInstancePath(ApiRoutes.PurchaseOrders.FULL_MOVEMENT_TEMPLATE, u.MovementId) }
                    }
                }),
                _links = new
                {
                    self = new { href = UrlHelper.FormatInstancePath(ApiRoutes.PurchaseOrders.FULL_BY_ID_TEMPLATE, order.Id) },
                    inventory = new { href = UrlHelper.FormatInstancePath(ApiRoutes.PurchaseOrders.FULL_LOCATION_FILTER_TEMPLATE, order.LocationId) }
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
                Type = ProblemDetailsConstants.Types.NOT_FOUND_ERROR,
                Title = ProblemDetailsConstants.Titles.NOT_FOUND,
                Status = 404,
                Detail = ex.Message,
                Instance = HttpContext.Request.Path,
                TraceId = HttpContext.TraceIdentifier
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("status"))
        {
            return Conflict(new
            {
                Type = ProblemDetailsConstants.Types.ORDER_ALREADY_REVIEWED,
                Title = ProblemDetailsConstants.Titles.ORDER_ALREADY_REVIEWED,
                Status = 409,
                Detail = ex.Message,
                Instance = HttpContext.Request.Path,
                TraceId = HttpContext.TraceIdentifier
            });
        }
    }

    /// <summary>
    /// POST /api/v1/purchase-orders/{id}/deny - Deny purchase order
    /// Authorization: Admin only
    /// </summary>
    [HttpPost(ApiRoutes.PurchaseOrders.DENY)]
    [Authorize(Roles = AuthorizationConstants.Roles.ADMIN)]
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
            var reviewedBy = _currentUserService.Username ?? RoleConstants.ADMIN;
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
                    self = new { href = UrlHelper.FormatInstancePath(ApiRoutes.PurchaseOrders.FULL_BY_ID_TEMPLATE, order.Id) }
                }
            };

            return Ok(response);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return NotFound(new
            {
                Type = ProblemDetailsConstants.Types.NOT_FOUND_ERROR,
                Title = ProblemDetailsConstants.Titles.NOT_FOUND,
                Status = 404,
                Detail = ex.Message,
                Instance = HttpContext.Request.Path,
                TraceId = HttpContext.TraceIdentifier
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("status"))
        {
            return Conflict(new
            {
                Type = ProblemDetailsConstants.Types.ORDER_ALREADY_REVIEWED,
                Title = ProblemDetailsConstants.Titles.ORDER_ALREADY_REVIEWED,
                Status = 409,
                Detail = ex.Message,
                Instance = HttpContext.Request.Path,
                TraceId = HttpContext.TraceIdentifier
            });
        }
    }

    /// <summary>
    /// GET /api/v1/purchase-orders/pending - Get pending orders (Admin dashboard)
    /// Authorization: Admin only
    /// </summary>
    [HttpGet(ApiRoutes.PurchaseOrders.PENDING)]
    [Authorize(Roles = AuthorizationConstants.Roles.ADMIN)]
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
                    self = new { href = UrlHelper.FormatInstancePath(ApiRoutes.PurchaseOrders.FULL_BY_ID_TEMPLATE, po.Id) },
                    confirm = new
                    {
                        href = UrlHelper.FormatInstancePath(ApiRoutes.PurchaseOrders.FULL_CONFIRM_TEMPLATE, po.Id),
                        method = HttpConstants.Methods.POST
                    },
                    deny = new
                    {
                        href = UrlHelper.FormatInstancePath(ApiRoutes.PurchaseOrders.FULL_DENY_TEMPLATE, po.Id),
                        method = HttpConstants.Methods.POST
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