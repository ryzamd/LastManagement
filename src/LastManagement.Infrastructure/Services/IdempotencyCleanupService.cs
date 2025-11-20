using LastManagement.Application.Features.PurchaseOrders.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LastManagement.Infrastructure.Services;

public sealed class IdempotencyCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<IdempotencyCleanupService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromHours(1);

    public IdempotencyCleanupService(IServiceProvider serviceProvider, ILogger<IdempotencyCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("IdempotencyCleanupService started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_interval, stoppingToken);

                using var scope = _serviceProvider.CreateScope();
                var idempotencyService = scope.ServiceProvider
                    .GetRequiredService<IIdempotencyService>();

                await idempotencyService.CleanupExpiredKeysAsync(stoppingToken);

                _logger.LogInformation("Expired idempotency keys cleaned up at {Time}", DateTime.UtcNow);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up expired idempotency keys");
            }
        }

        _logger.LogInformation("IdempotencyCleanupService stopped");
    }
}