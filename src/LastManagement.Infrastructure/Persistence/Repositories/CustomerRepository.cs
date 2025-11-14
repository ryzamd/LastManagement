using LastManagement.Application.Features.Customers.Interfaces;
using LastManagement.Domain.Customers;
using Microsoft.EntityFrameworkCore;

namespace LastManagement.Infrastructure.Persistence.Repositories;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly ApplicationDbContext _context;

    public CustomerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Customer>().AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Customer?> GetByIdForUpdateAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Customer>().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<(List<Customer> Items, int TotalCount)> GetPagedAsync(int limit, int? afterId, string? filterStatus, string? orderBy, CancellationToken cancellationToken = default)
    {
        var query = _context.Set<Customer>().AsNoTracking();

        // Filter by status
        if (!string.IsNullOrEmpty(filterStatus) && Enum.TryParse<CustomerStatus>(filterStatus, out var status))
        {
            query = query.Where(c => c.Status == status);
        }

        // Cursor pagination
        if (afterId.HasValue)
        {
            query = query.Where(c => c.Id > afterId.Value);
        }

        // Order by
        query = orderBy?.ToLower() switch
        {
            "customername asc" => query.OrderBy(c => c.CustomerName),
            "customername desc" => query.OrderByDescending(c => c.CustomerName),
            "createdat desc" => query.OrderByDescending(c => c.CreatedAt),
            "createdat asc" => query.OrderBy(c => c.CreatedAt),
            _ => query.OrderBy(c => c.Id) // Default
        };

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Take(limit).ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> ExistsByNameAsync(string customerName, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Set<Customer>().AsNoTracking().Where(c => c.CustomerName == customerName);

        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> HasAssociatedLastsAsync(int customerId, CancellationToken cancellationToken = default)
    {
        // TODO: Implement when LastNames table is ready
        // For now, return false
        return await Task.FromResult(false);
    }

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await _context.Set<Customer>().AddAsync(customer, cancellationToken);
    }

    public void Update(Customer customer)
    {
        _context.Set<Customer>().Update(customer);
    }

    public void Delete(Customer customer)
    {
        _context.Set<Customer>().Remove(customer);
    }
}