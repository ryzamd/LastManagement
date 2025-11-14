using LastManagement.Application.Common.Interfaces;
using LastManagement.Application.Features.Authentication.Interfaces;
using LastManagement.Application.Features.Customers.Interfaces;
using LastManagement.Infrastructure.Authentication;
using LastManagement.Infrastructure.Options;
using LastManagement.Infrastructure.Persistence;
using LastManagement.Infrastructure.Persistence.Interceptors;
using LastManagement.Infrastructure.Persistence.Repositories;
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
                    configuration.GetConnectionString("DefaultConnection"),
                    npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
                )
                .AddInterceptors(interceptor);
        });

        // Repositories
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();

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

        return services;
    }
}