using LastManagement.Domain.Customers;

namespace LastManagement.Application.Features.Customers.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Customer?> GetByIdForUpdateAsync(int id, CancellationToken cancellationToken = default);
    Task<(List<Customer> Items, int TotalCount)> GetPagedAsync(int limit, int? afterId, string? filterStatus, string? orderBy, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string customerName, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> HasAssociatedLastsAsync(int customerId, CancellationToken cancellationToken = default);
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
    void Update(Customer customer);
    void Delete(Customer customer);
}