namespace AspNetCore.Multitenancy;

/// <summary>
/// Extension methods for registering multi-tenancy services with the DI container
/// and adding the tenant middleware to the pipeline.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the core multi-tenancy services and returns a <see cref="MultitenancyBuilder"/>
    /// for further configuration via <c>With*</c> extension methods.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.Services
    ///     .AddMultitenancy(options =>
    ///     {
    ///         options.TenantStore = TenantStore.InMemory;
    ///         options.IsolationStrategy = IsolationStrategy.RowLevel;
    ///     })
    ///     .WithHeaderResolver("X-Tenant-Id")
    ///     .WithEFCoreIsolation&lt;AppDbContext&gt;();
    /// </code>
    /// </example>
    public static MultitenancyBuilder AddMultitenancy(
        this IServiceCollection services,
        Action<MultitenancyOptions>? configure = null)
    {
        var options = new MultitenancyOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);
        services.AddScoped<ITenantContext, TenantContext>();

        // Register default in-memory store unless overridden later
        if (options.TenantStore == TenantStore.InMemory)
        {
            services.AddSingleton<ITenantStore, InMemoryTenantStore>();
        }

        return new MultitenancyBuilder(services, options);
    }

    /// <summary>
    /// Adds the <see cref="TenantMiddleware"/> to the ASP.NET Core pipeline.
    /// Call this in <c>app.Use*()</c> — before any routing or authorization middleware.
    /// </summary>
    public static IApplicationBuilder UseMultitenancy(this IApplicationBuilder app)
        => app.UseMiddleware<TenantMiddleware>();
}
