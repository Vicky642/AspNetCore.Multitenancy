namespace AspNetCore.Multitenancy.FeatureFlags;

/// <summary>
/// Maps subscription plan names to sets of enabled features.
/// Configure the plan → feature mapping at startup.
///
/// Example:
/// <code>
/// services.Configure&lt;PlanFeatureOptions&gt;(opts =>
/// {
///     opts.AddPlan("free",       new[] { "BasicDashboard" });
///     opts.AddPlan("pro",        new[] { "BasicDashboard", "AdvancedReporting", "ApiAccess" });
///     opts.AddPlan("enterprise", new[] { "BasicDashboard", "AdvancedReporting", "ApiAccess", "WhiteLabel", "SSOIntegration" });
/// });
/// </code>
/// </summary>
public class TenantPlanFeatureProvider : ITenantFeatureService
{
    private readonly ITenantContext _tenantContext;
    private readonly PlanFeatureOptions _options;

    public TenantPlanFeatureProvider(ITenantContext tenantContext, PlanFeatureOptions options)
    {
        _tenantContext = tenantContext;
        _options = options;
    }

    /// <inheritdoc />
    public bool IsEnabled(string featureName)
    {
        var features = GetEnabledFeatures();
        return features.Contains(featureName);
    }

    /// <inheritdoc />
    public IReadOnlySet<string> GetEnabledFeatures()
    {
        if (!_tenantContext.HasTenant)
            return new HashSet<string>();

        var plan = _tenantContext.CurrentTenant.Plan ?? "free";
        return _options.GetFeaturesForPlan(plan);
    }
}

/// <summary>
/// Configuration object that maps plan names to feature sets.
/// </summary>
public class PlanFeatureOptions
{
    private readonly Dictionary<string, HashSet<string>> _planFeatures = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Registers the features available for a subscription plan.
    /// </summary>
    public PlanFeatureOptions AddPlan(string planName, IEnumerable<string> features)
    {
        _planFeatures[planName] = new HashSet<string>(features, StringComparer.OrdinalIgnoreCase);
        return this;
    }

    internal IReadOnlySet<string> GetFeaturesForPlan(string plan)
        => _planFeatures.TryGetValue(plan, out var features)
            ? features
            : new HashSet<string>();
}
