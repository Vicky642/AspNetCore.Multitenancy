using Hangfire;

namespace AspNetCore.Multitenancy.BackgroundJobs;

/// <summary>
/// Schedules Hangfire recurring jobs for every active tenant automatically.
/// Use this instead of <see cref="RecurringJob"/> directly to ensure jobs
/// run once per tenant with a properly populated <see cref="ITenantContext"/>.
/// </summary>
public class TenantRecurringJobManager
{
    private readonly ITenantStore _tenantStore;
    private readonly IRecurringJobManager _hangfireJobManager;

    public TenantRecurringJobManager(ITenantStore tenantStore, IRecurringJobManager hangfireJobManager)
    {
        _tenantStore = tenantStore;
        _hangfireJobManager = hangfireJobManager;
    }

    /// <summary>
    /// Schedules <typeparamref name="TJob"/> as a recurring Hangfire job for every active tenant.
    /// Each tenant gets its own job entry: <c>{jobId}-{tenantId}</c>.
    /// </summary>
    /// <typeparam name="TJob">The Hangfire job type. Must have an <c>Execute()</c> or <c>ExecuteAsync()</c> method.</typeparam>
    /// <param name="jobId">Base job identifier (e.g., "daily-report").</param>
    /// <param name="cron">Cron expression (e.g., <see cref="Cron.Daily"/>).</param>
    /// <param name="methodSelector">
    /// Expression pointing to the job method.
    /// Example: <c>job => job.ExecuteAsync()</c>
    /// </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task ScheduleForAllAsync<TJob>(
        string jobId,
        string cron,
        System.Linq.Expressions.Expression<Func<TJob, Task>> methodSelector,
        CancellationToken cancellationToken = default)
    {
        var tenants = await _tenantStore.GetAllTenantsAsync(cancellationToken);

        foreach (var tenant in tenants.Where(t => t.IsActive))
        {
            var tenantJobId = $"{jobId}-{tenant.Id}";
            _hangfireJobManager.AddOrUpdate(tenantJobId, methodSelector, cron);
        }
    }

    /// <summary>
    /// Removes all recurring job entries for the given job ID across all tenants.
    /// </summary>
    public async Task RemoveForAllAsync(string jobId, CancellationToken cancellationToken = default)
    {
        var tenants = await _tenantStore.GetAllTenantsAsync(cancellationToken);

        foreach (var tenant in tenants)
        {
            _hangfireJobManager.RemoveIfExists($"{jobId}-{tenant.Id}");
        }
    }
}
