using Hangfire;

namespace AspNetCore.Multitenancy.BackgroundJobs;

/// <summary>
/// Extension methods on <see cref="MultitenancyBuilder"/> for Hangfire integration.
/// </summary>
public static class HangfireExtensions
{
    /// <summary>
    /// Registers tenant-aware Hangfire services:
    /// <list type="bullet">
    ///   <item><see cref="TenantJobFilter"/> — propagates tenant context into jobs</item>
    ///   <item><see cref="TenantJobActivator"/> — resolves jobs from per-tenant DI scope</item>
    ///   <item><see cref="TenantRecurringJobManager"/> — schedules jobs per tenant</item>
    /// </list>
    /// <para>
    /// You still need to call <c>services.AddHangfire(...)</c> and <c>services.AddHangfireServer()</c>
    /// separately with your storage configuration (SQL Server, Redis, etc.).
    /// </para>
    /// </summary>
    public static MultitenancyBuilder WithHangfireJobs(this MultitenancyBuilder builder)
    {
        builder.Services.AddScoped<TenantJobFilter>();
        builder.Services.AddSingleton<TenantJobActivator>();
        builder.Services.AddScoped<TenantRecurringJobManager>();

        return builder;
    }

    /// <summary>
    /// Adds <see cref="TenantJobFilter"/> to the Hangfire global configuration pipeline.
    /// Call this inside your <c>AddHangfire</c> configuration block:
    /// <code>
    /// services.AddHangfire((sp, config) =&gt;
    /// {
    ///     config.UseSqlServerStorage(...);
    ///     config.UseTenantJobFilter(sp);
    /// });
    /// </code>
    /// </summary>
    public static IGlobalConfiguration UseTenantJobFilter(this IGlobalConfiguration configuration, IServiceProvider serviceProvider)
    {
        var filter = serviceProvider.GetRequiredService<TenantJobFilter>();
        configuration.UseFilter(filter);
        return configuration;
    }
}
