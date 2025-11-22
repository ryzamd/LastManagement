using LastManagement.Api.Constants;
using LastManagement.Application.Common.Interfaces;
using LastManagement.Application.Features.Authentication.Interfaces;
using LastManagement.Application.Features.Customers.Interfaces;
using LastManagement.Application.Features.InventoryStocks.Interfaces;
using LastManagement.Application.Features.LastModels.Interfaces;
using LastManagement.Application.Features.LastNames.Interfaces;
using LastManagement.Application.Features.LastSizes.Interfaces;
using LastManagement.Application.Features.Locations.Interfaces;
using LastManagement.Application.Features.PurchaseOrders.Interfaces;
using LastManagement.Infrastructure.Authentication;
using LastManagement.Infrastructure.Options;
using LastManagement.Infrastructure.Persistence;
using LastManagement.Infrastructure.Persistence.Interceptors;
using LastManagement.Infrastructure.Persistence.Repositories;
using LastManagement.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LastManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddScoped<AuditInterceptor>();
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<AuditInterceptor>();
            options.UseNpgsql(
                    configuration.GetConnectionString(ConfigurationKeys.Database.DEFAULT_CONNECTION),
                    npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
                ).AddInterceptors(interceptor);
        });

        // Repositories
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<ILastSizeRepository, LastSizeRepository>();
        services.AddScoped<ILastNameRepository, LastNameRepository>();
        services.AddScoped<IInventoryMovementRepository, InventoryMovementRepository>();
        services.AddScoped<IInventoryStockRepository, InventoryStockRepository>();
        services.AddScoped<ILastModelRepository, LastModelRepository>();
        services.AddScoped<IInventoryStockRepository, InventoryStockRepository>();
        services.AddScoped<IInventoryMovementRepository, InventoryMovementRepository>();
        services.AddScoped<ILastNameRepository, LastNameRepository>();
        // Purchase Orders
        services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
        services.AddScoped<IPurchaseOrderItemRepository, PurchaseOrderItemRepository>();
        services.AddScoped<IIdempotencyService, IdempotencyService>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // JWT
        services.Configure<JwtOptions>(options => configuration.GetSection(JwtOptions.SectionName).Bind(options));
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // Password Hasher
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        // Current User
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Background Services
        services.AddHostedService<IdempotencyCleanupService>();

        return services;
    }
}