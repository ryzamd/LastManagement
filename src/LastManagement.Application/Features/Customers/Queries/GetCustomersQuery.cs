using LastManagement.Api.Constants;
using LastManagement.Api.Global.Helpers;
using LastManagement.Application.Common.Models;
using LastManagement.Application.Features.Customers.DTOs;
using LastManagement.Application.Features.Customers.Interfaces;
using LastManagement.Utilities.Helpers;

namespace LastManagement.Application.Features.Customers.Queries;

public sealed record GetCustomersQuery(int Limit, int? AfterId, string? FilterStatus, string? OrderBy);

public sealed class GetCustomersQueryHandler
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomersQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<PaginatedResponse<CustomerDto>>> HandleAsync(GetCustomersQuery query, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _customerRepository.GetPagedAsync(
            query.Limit,
            query.AfterId,
            query.FilterStatus,
            query.OrderBy,
            cancellationToken);

        var dtos = items.Select(c => new CustomerDto
        {
            Id = c.Id,
            CustomerName = c.CustomerName,
            Status = c.Status.ToString(),
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt,
            Version = c.Version
        }).ToList();

        string? nextLink = null;
        if (items.Count == query.Limit)
        {
            var lastId = items[^1].Id;
            nextLink = UrlHelper.FormatResourceUrl(ApiRoutes.Customers.FULL_PAGINATION_TEMPLATE, query.Limit, lastId);

            if (!string.IsNullOrEmpty(query.FilterStatus))
                nextLink += StringFormatter.FormatMessage(ApiRoutes.Customers.QueryStrings.FILTER_STATUS, query.FilterStatus);

            if (!string.IsNullOrEmpty(query.OrderBy))
                nextLink += StringFormatter.FormatMessage(ApiRoutes.Customers.QueryStrings.ORDERBY, query.OrderBy);
        }

        var response = new PaginatedResponse<CustomerDto>
        {
            Value = dtos,
            NextLink = nextLink,
            Count = totalCount
        };

        return Result.Success(response);
    }
}