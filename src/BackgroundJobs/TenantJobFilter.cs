using Hangfire;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;

namespace AspNetCore.Multitenancy.BackgroundJobs;

/// <summary>
/// Hangfire server filter that automatically propagates tenant context into background jobs.
///
/// On enqueue: serializes the current <c>TenantId</c> into the Hangfire job parameters.
/// On execute: reads <c>TenantId</c> from job params, creates a tenant DI scope,
///             sets <see cref="ITenantContext"/>, then executes the job in that scope.
///
/// Register once in Program.cs — jobs see a fully populated ITenantContext identical
/// to an HTTP request context.
/// </summary>
public class TenantJobFilter : JobFilterAttribute, IClientFilter, IServerFilter
{
    private const string TenantIdKey = "TenantId";

    private readonly ITenantStore _tenantStore;
    private readonly IServiceProvider _serviceProvider;

    public TenantJobFilter(ITenantStore tenantStore, IServiceProvider serviceProvider)
    {
        _tenantStore = tenantStore;
        _serviceProvider = serviceProvider;
    }

    // ── Client side (enqueue) ──────────────────────────────────────────────

    /// <summary>Serializes the current tenant ID into the Hangfire job parameters at enqueue time.</summary>
    public void OnCreating(CreatingContext filterContext)
    {
        var tenantContext = _serviceProvider.GetService<ITenantContext>();
        if (tenantContext?.HasTenant == true)
        {
            filterContext.SetJobParameter(TenantIdKey, tenantContext.CurrentTenant.Id);
        }
    }

    public void OnCreated(CreatedContext filterContext) { }

    // ── Server side (execute) ──────────────────────────────────────────────

    /// <summary>Restores the tenant context before executing the job.</summary>
    public void OnPerforming(PerformingContext filterContext)
    {
        var tenantId = filterContext.GetJobParameter<string>(TenantIdKey);
        if (string.IsNullOrWhiteSpace(tenantId))
            return;

        var tenant = _tenantStore.GetTenantAsync(tenantId).GetAwaiter().GetResult();
        if (tenant is null)
            return;

        // Create a scoped tenant context and store it on the filter context items
        // so OnPerformed can dispose it
        var scope = _serviceProvider.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<ITenantContext>() as TenantContext;
        ctx?.SetTenant(tenant);

        filterContext.Items["TenantScope"] = scope;
        filterContext.Items["TenantContext"] = ctx;
    }

    /// <summary>Disposes the tenant scope after job execution.</summary>
    public void OnPerformed(PerformedContext filterContext)
    {
        if (filterContext.Items.TryGetValue("TenantScope", out var scope) && scope is IServiceScope s)
            s.Dispose();
    }
}
