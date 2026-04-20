using AspNetCore.Multitenancy;
using Microsoft.Extensions.Logging;

namespace SaaSDemo.Jobs;

/// <summary>
/// Background job that purges old/inactive records for the current tenant.
/// Safe to run weekly or daily — tenant isolation is automatic.
/// </summary>
public class CleanupJob
{
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<CleanupJob> _logger;

    public CleanupJob(ITenantContext tenantContext, ILogger<CleanupJob> logger)
    {
        _tenantContext = tenantContext;
        _logger = logger;
    }

    /// <summary>
    /// Executes cleanup for the current tenant.
    /// Tenant context is set automatically by TenantJobFilter before this is called.
    /// </summary>
    public Task ExecuteAsync()
    {
        var tenantId = _tenantContext.HasTenant
            ? _tenantContext.CurrentTenant.Id
            : "(no tenant)";

        _logger.LogInformation(
            "[Cleanup] Purging stale records for tenant '{TenantId}' at {Time}",
            tenantId,
            DateTimeOffset.UtcNow);

        // In a real app: DELETE inactive products older than 90 days, purge logs, etc.

        return Task.CompletedTask;
    }
}
