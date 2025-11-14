using LastManagement.Application.Common.Models;
using LastManagement.Application.Features.Customers.DTOs;
using LastManagement.Application.Features.Customers.Interfaces;

namespace LastManagement.Application.Features.Customers.Queries;

public sealed record GetCustomerByIdQuery(int Id);

public sealed class GetCustomerByIdQueryHandler
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<CustomerDto>> HandleAsync(GetCustomerByIdQuery query, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(query.Id, cancellationToken);
        if (customer == null)
        {
            return Result.Failure<CustomerDto>("Customer not found");
        }

        var dto = new CustomerDto
        {
            Id = customer.Id,
            CustomerName = customer.CustomerName,
            Status = customer.Status.ToString(),
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt,
            Version = customer.Version
        };

        return Result.Success(dto);
    }
}