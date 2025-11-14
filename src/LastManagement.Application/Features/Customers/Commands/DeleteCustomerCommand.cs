using LastManagement.Application.Common.Interfaces;
using LastManagement.Application.Common.Models;
using LastManagement.Application.Features.Customers.Interfaces;

namespace LastManagement.Application.Features.Customers.Commands;

public sealed record DeleteCustomerCommand(int Id);

public sealed class DeleteCustomerCommandHandler
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(DeleteCustomerCommand command, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdForUpdateAsync(command.Id, cancellationToken);
        if (customer == null)
        {
            return Result.Failure("Customer not found");
        }

        // Check if has associated lasts
        if (await _customerRepository.HasAssociatedLastsAsync(command.Id, cancellationToken))
        {
            return Result.Failure("Cannot delete customer because it has associated lasts");
        }

        _customerRepository.Delete(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}