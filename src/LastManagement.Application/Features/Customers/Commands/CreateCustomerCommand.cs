using LastManagement.Application.Common.Interfaces;
using LastManagement.Application.Common.Models;
using LastManagement.Application.Features.Customers.DTOs;
using LastManagement.Application.Features.Customers.Interfaces;
using LastManagement.Domain.Customers;

namespace LastManagement.Application.Features.Customers.Commands;

public sealed record CreateCustomerCommand(string CustomerName, string Status);

public sealed class CreateCustomerCommandHandler
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CustomerDto>> HandleAsync(CreateCustomerCommand command, CancellationToken cancellationToken = default)
    {
        // Check duplicate name
        if (await _customerRepository.ExistsByNameAsync(command.CustomerName, null, cancellationToken))
        {
            return Result.Failure<CustomerDto>($"Customer with name '{command.CustomerName}' already exists");
        }

        // Parse status
        if (!Enum.TryParse<CustomerStatus>(command.Status, out var status))
        {
            return Result.Failure<CustomerDto>("Invalid status value");
        }

        // Create entity
        var customer = Customer.Create(command.CustomerName, status);

        await _customerRepository.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Map to DTO
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