using LastManagement.Application.Common.Interfaces;
using LastManagement.Application.Common.Models;
using LastManagement.Application.Features.Customers.DTOs;
using LastManagement.Application.Features.Customers.Interfaces;
using LastManagement.Domain.Customers;
using LastManagement.Utilities.Constants.Global;
using LastManagement.Utilities.Helpers;

namespace LastManagement.Application.Features.Customers.Commands;

public sealed record UpdateCustomerCommand(
    int Id,
    string? CustomerName,
    string? Status,
    int ExpectedVersion);

public sealed class UpdateCustomerCommandHandler
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CustomerDto>> HandleAsync(UpdateCustomerCommand command, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdForUpdateAsync(command.Id, cancellationToken);
        if (customer == null)
        {
            return Result.Failure<CustomerDto>(ResultMessages.Customer.NOT_FOUND);
        }

        // Check version for concurrency
        if (customer.Version != command.ExpectedVersion)
        {
            return Result.Failure<CustomerDto>(ResultMessages.Customer.MODIFIED_BY_ANOTHER);
        }

        // Update name if provided
        if (!string.IsNullOrWhiteSpace(command.CustomerName))
        {
            // Check duplicate
            if (await _customerRepository.ExistsByNameAsync(command.CustomerName, customer.Id, cancellationToken))
            {
                return Result.Failure<CustomerDto>(StringFormatter.FormatValidation(ResultMessages.Customer.CUSTOMER_ALREADY_EXISTS, command.CustomerName));
            }
            customer.UpdateName(command.CustomerName);
        }

        // Update status if provided
        if (!string.IsNullOrWhiteSpace(command.Status))
        {
            if (!Enum.TryParse<CustomerStatus>(command.Status, out var newStatus))
            {
                return Result.Failure<CustomerDto>(ResultMessages.Customer.INVALID_STATUS);
            }
            customer.UpdateStatus(newStatus);
        }

        _customerRepository.Update(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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