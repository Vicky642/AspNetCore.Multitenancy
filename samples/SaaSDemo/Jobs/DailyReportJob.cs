using AspNetCore.Multitenancy;
using Microsoft.Extensions.Logging;

namespace SaaSDemo.Jobs;

/// <summary>
/// Runs once per day for every tenant to generate a daily summary report.
/// Tenant context is automatically populated by TenantJobFilter.
/// </summary>
public class DailyReportJob
{
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<DailyReportJob> _logger;

    public DailyReportJob(ITenantContext tenantContext, ILogger<DailyReportJob> logger)
    {
        _tenantContext = tenantContext;
        _logger = logger;
    }

    /// <summary>
    /// Executes the daily report for the current tenant.
    /// Hangfire calls this; TenantJobFilter ensures ITenantContext is populated.
    /// </summary>
    public Task ExecuteAsync()
    {
        var tenantId = _tenantContext.HasTenant
            ? _tenantContext.CurrentTenant.Id
            : "(no tenant)";

        _logger.LogInformation(
            "[DailyReport] Generating daily report for tenant '{TenantId}' at {Time}",
            tenantId,
            DateTimeOffset.UtcNow);

        // In a real app: query the tenant's DB, build summaries, send email, etc.

        return Task.CompletedTask;
    }
}
