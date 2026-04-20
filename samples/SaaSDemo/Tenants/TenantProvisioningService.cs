using AspNetCore.Multitenancy;
using Microsoft.Extensions.Logging;

namespace SaaSDemo.Tenants;

/// <summary>
/// Provisions infrastructure for a newly created tenant.
/// In a real SaaS app this would: create the DB/schema, seed default data,
/// send a welcome email, set up billing, etc.
/// </summary>
public class TenantProvisioningService
{
    private readonly ILogger<TenantProvisioningService> _logger;

    public TenantProvisioningService(ILogger<TenantProvisioningService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Runs all provisioning steps for the given tenant.
    /// </summary>
    public async Task ProvisionAsync(TenantInfo tenant)
    {
        _logger.LogInformation("Provisioning tenant '{TenantId}' (plan: {Plan})...", tenant.Id, tenant.Plan);

        await CreateDatabaseOrSchemaAsync(tenant);
        await SeedDefaultDataAsync(tenant);

        _logger.LogInformation("Tenant '{TenantId}' provisioned successfully.", tenant.Id);
    }

    private Task CreateDatabaseOrSchemaAsync(TenantInfo tenant)
    {
        // In a real app:
        //   - DB-per-tenant: run EF migrations against tenant.ConnectionString
        //   - Schema-per-tenant: CREATE SCHEMA {tenant.Schema}; run migrations
        //   - Row-level: nothing to do — migrations already applied at startup
        _logger.LogDebug("Creating database/schema for tenant '{TenantId}'...", tenant.Id);
        return Task.CompletedTask;
    }

    private Task SeedDefaultDataAsync(TenantInfo tenant)
    {
        _logger.LogDebug("Seeding default data for tenant '{TenantId}'...", tenant.Id);
        return Task.CompletedTask;
    }
}
