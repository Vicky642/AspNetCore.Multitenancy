using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace AspNetCore.Multitenancy.EntityFrameworkCore;

/// <summary>
/// Runs EF Core migrations for each tenant automatically.
/// Use during application startup or tenant provisioning to ensure all
/// tenant databases/schemas are up-to-date.
/// </summary>
public class TenantSchemaMigrator<TContext> where TContext : DbContext
{
    private readonly ITenantStore _tenantStore;
    private readonly TenantDbContextFactory<TContext> _contextFactory;
    private readonly ILogger<TenantSchemaMigrator<TContext>> _logger;

    public TenantSchemaMigrator(
        ITenantStore tenantStore,
        TenantDbContextFactory<TContext> contextFactory,
        ILogger<TenantSchemaMigrator<TContext>> logger)
    {
        _tenantStore = tenantStore;
        _contextFactory = contextFactory;
        _logger = logger;
    }

    /// <summary>
    /// Runs pending EF Core migrations for all active tenants.
    /// Safe to call on every startup — EF Core migrations are idempotent.
    /// </summary>
    public async Task MigrateAllTenantsAsync(CancellationToken cancellationToken = default)
    {
        var tenants = await _tenantStore.GetAllTenantsAsync(cancellationToken);

        foreach (var tenant in tenants.Where(t => t.IsActive))
        {
            await MigrateTenantAsync(tenant, cancellationToken);
        }
    }

    /// <summary>
    /// Runs pending migrations for a single tenant.
    /// </summary>
    public async Task MigrateTenantAsync(TenantInfo tenant, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Migrating database for tenant '{TenantId}'...", tenant.Id);

        try
        {
            await using var context = _contextFactory.CreateContext();
            await context.Database.MigrateAsync(cancellationToken);
            _logger.LogInformation("Migration complete for tenant '{TenantId}'.", tenant.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Migration failed for tenant '{TenantId}'.", tenant.Id);
            throw;
        }
    }
}
