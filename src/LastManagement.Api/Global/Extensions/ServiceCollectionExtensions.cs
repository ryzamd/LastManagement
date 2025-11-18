using LastManagement.Api.Features.Authentication;
using LastManagement.Api.Features.LastNames;
using LastManagement.Application.Features.Authentication.Commands;
using LastManagement.Application.Features.Authentication.Queries;
using LastManagement.Application.Features.Customers.Commands;
using LastManagement.Application.Features.Customers.Queries;
using LastManagement.Application.Features.InventoryStocks.Commands;
using LastManagement.Application.Features.InventoryStocks.Interfaces;
using LastManagement.Application.Features.InventoryStocks.Queries;
using LastManagement.Application.Features.LastModels.Queries;
using LastManagement.Application.Features.LastNames.Commands;
using LastManagement.Application.Features.LastNames.Interfaces;
using LastManagement.Application.Features.LastNames.Queries;
using LastManagement.Application.Features.LastSizes.Commands;
using LastManagement.Application.Features.LastSizes.Queries;
using LastManagement.Application.Features.Locations.Commands;
using LastManagement.Application.Features.Locations.Queries;
using LastManagement.Infrastructure.Persistence.Repositories;

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
        services.AddScoped<CreateLastSizeBatchCommand>();
        services.AddScoped<UpdateLastSizeBatchCommand>();
        services.AddScoped<DeleteLastSizeBatchCommand>();
        // Inventory Stock
        services.AddScoped<IInventoryStockRepository, InventoryStockRepository>();
        services.AddScoped<IInventoryMovementRepository, InventoryMovementRepository>();
        services.AddScoped<AdjustStockCommand>();
        services.AddScoped<TransferStockCommand>();
        services.AddScoped<BatchAdjustStockCommand>();
        services.AddScoped<GetInventoryStocksQuery>();
        services.AddScoped<GetInventoryStockByIdQuery>();
        // Last Names
        services.AddScoped<ILastNameRepository, LastNameRepository>();
        services.AddScoped<CreateLastNameCommand>();
        services.AddScoped<UpdateLastNameCommand>();
        services.AddScoped<UpdateLastNameBatchCommand>();
        services.AddScoped<GetLastNamesQuery>();
        services.AddScoped<GetLastNameByIdQuery>();
        // LastModels
        services.AddScoped<GetLastModelsQuery>();
        services.AddScoped<GetModelsByLastIdQuery>();

        return services;
    }

    public static IServiceCollection AddMappings(this IServiceCollection services)
    {
        // Configure Mapster
        AuthenticationMapping.ConfigureMappings();
        LastNamesMapping.ConfigureMappings();

        return services;
    }
}