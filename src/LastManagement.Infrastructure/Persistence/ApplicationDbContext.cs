using LastManagement.Domain.Accounts;
using LastManagement.Domain.Common;
using LastManagement.Domain.Customers;
using LastManagement.Domain.InventoryStocks;
using LastManagement.Domain.LastModels.Entities;
using LastManagement.Domain.LastNames.Entities;
using LastManagement.Domain.LastSizes;
using LastManagement.Domain.Locations;
using Microsoft.EntityFrameworkCore;

namespace LastManagement.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Account> AccountsRepository => Set<Account>();
    public DbSet<Customer> CustomersRepository => Set<Customer>();
    public DbSet<Location> LocationsRepository => Set<Location>();
    public DbSet<LastSize> LastSizesRepository => Set<LastSize>();
    public DbSet<LastName> LastNameRepository => Set<LastName>();
    public DbSet<LastModel> LastModelsRepository => Set<LastModel>();
    public DbSet<InventoryStock> InventoryStocksRepository => Set<InventoryStock>();
    public DbSet<InventoryMovement> InventoryMovementsRepository => Set<InventoryMovement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Ignore<Entity>();
        modelBuilder.Ignore<DomainEvent>();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update timestamps
        var entries = ChangeTracker.Entries<Entity>()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            entry.Entity.IncrementVersion();
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}