using LastManagement.Api.Features.Authentication;
using LastManagement.Application.Features.Authentication.Commands;
using LastManagement.Application.Features.Authentication.Queries;
using LastManagement.Application.Features.Customers.Commands;
using LastManagement.Application.Features.Customers.Queries;
using LastManagement.Application.Features.LastSizes.Commands;
using LastManagement.Application.Features.LastSizes.Queries;
using LastManagement.Application.Features.Locations.Commands;
using LastManagement.Application.Features.Locations.Queries;

namespace LastManagement.Api.Global.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationHandlers(this IServiceCollection services)
    {
        // Authentication handlers
        services.AddScoped<LoginCommandHandler>();
        services.AddScoped<RefreshTokenCommandHandler>();
        services.AddScoped<LogoutCommandHandler>();
        services.AddScoped<GetCurrentUserQueryHandler>();
        // Customers
        services.AddScoped<GetCustomersQueryHandler>();
        services.AddScoped<GetCustomerByIdQueryHandler>();
        services.AddScoped<CreateCustomerCommandHandler>();
        services.AddScoped<UpdateCustomerCommandHandler>();
        services.AddScoped<DeleteCustomerCommandHandler>();
        // Locations
        services.AddScoped<GetLocationsQueryHandler>();
        services.AddScoped<GetLocationByIdQueryHandler>();
        services.AddScoped<CreateLocationCommandHandler>();
        services.AddScoped<UpdateLocationCommandHandler>();
        services.AddScoped<DeleteLocationCommandHandler>();
        // LastSize
        services.AddScoped<GetLastSizesQuery>();
        services.AddScoped<GetLastSizeByIdQuery>();
        services.AddScoped<CreateLastSizeCommand>();
        services.AddScoped<UpdateLastSizeCommand>();
        services.AddScoped<DeleteLastSizeCommand>();

        return services;
    }

    public static IServiceCollection AddMappings(this IServiceCollection services)
    {
        // Configure Mapster
        AuthenticationMapping.ConfigureMappings();

        return services;
    }
}