using Microsoft.Extensions.Hosting;

namespace AspNetCore.Multitenancy.BackgroundJobs;

/// <summary>
/// Base class for tenant-aware hosted services (<see cref="IHostedService"/>).
/// Override <see cref="ExecuteForTenantAsync"/> to implement per-tenant logic.
/// The base class iterates all active tenants and calls your method in a scoped tenant context.
/// </summary>
public abstract class TenantBackgroundService : BackgroundService
{
    private readonly ITenantStore _tenantStore;
    private readonly ITenantContext _tenantContext;
    private readonly ILogger _logger;

    protected TenantBackgroundService(
        ITenantStore tenantStore,
        ITenantContext tenantContext,
        ILogger logger)
    {
        _tenantStore = tenantStore;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var tenants = await _tenantStore.GetAllTenantsAsync(stoppingToken);

            foreach (var tenant in tenants.Where(t => t.IsActive))
            {
                using var scope = _tenantContext.CreateTenantScope(tenant.Id);
                var scopedCtx = scope.ServiceProvider.GetRequiredService<ITenantContext>();

                try
                {
                    await ExecuteForTenantAsync(scopedCtx, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error executing {Service} for tenant '{TenantId}'",
                        GetType().Name, tenant.Id);
                }
            }

            await DelayAsync(stoppingToken);
        }
    }

    /// <summary>
    /// Override to implement per-tenant background work.
    /// Called once per active tenant on each execution cycle.
    /// </summary>
    /// <param name="tenantContext">The tenant context for the current tenant.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    protected abstract Task ExecuteForTenantAsync(ITenantContext tenantContext, CancellationToken cancellationToken);

    /// <summary>
    /// Override to control how long to wait between execution cycles.
    /// Default: 1 minute.
    /// </summary>
    protected virtual Task DelayAsync(CancellationToken cancellationToken)
        => Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
}
