namespace AspNetCore.Multitenancy.FeatureFlags;

/// <summary>
/// Extension methods for registering feature flag services and middleware.
/// </summary>
public static class FeatureFlagExtensions
{
    /// <summary>
    /// Registers the per-tenant feature flag services.
    /// </summary>
    /// <param name="builder">The multitenancy builder.</param>
    /// <param name="configurePlans">
    /// A delegate to map subscription plans to feature sets.
    /// <code>
    /// .WithFeatureFlags(plans =>
    /// {
    ///     plans.AddPlan("free",       new[] { "BasicDashboard" });
    ///     plans.AddPlan("pro",        new[] { "BasicDashboard", "AdvancedReporting" });
    ///     plans.AddPlan("enterprise", new[] { "BasicDashboard", "AdvancedReporting", "WhiteLabel" });
    /// })
    /// </code>
    /// </param>
    public static MultitenancyBuilder WithFeatureFlags(
        this MultitenancyBuilder builder,
        Action<PlanFeatureOptions> configurePlans)
    {
        var options = new PlanFeatureOptions();
        configurePlans(options);

        builder.Services.AddSingleton(options);
        builder.Services.AddScoped<ITenantFeatureService, TenantPlanFeatureProvider>();

        return builder;
    }

    /// <summary>
    /// Adds the <see cref="TenantFeatureMiddleware"/> to the ASP.NET Core pipeline.
    /// Place this after <c>UseRouting()</c> and <c>UseMultitenancy()</c>.
    /// </summary>
    public static IApplicationBuilder UseTenantFeatures(this IApplicationBuilder app)
        => app.UseMiddleware<TenantFeatureMiddleware>();
}
