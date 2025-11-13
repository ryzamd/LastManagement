using LastManagement.Api.Features.Authentication;
using LastManagement.Application.Features.Authentication.Commands;
using LastManagement.Application.Features.Authentication.Queries;

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

        return services;
    }

    public static IServiceCollection AddMappings(this IServiceCollection services)
    {
        // Configure Mapster
        AuthenticationMapping.ConfigureMappings();

        return services;
    }
}