namespace AspNetCore.Multitenancy.FeatureFlags;

/// <summary>
/// Checks whether a named feature is enabled for the current tenant.
/// Features are gated by the tenant's subscription plan.
/// </summary>
public interface ITenantFeatureService
{
    /// <summary>
    /// Returns <c>true</c> if the named feature is enabled for the current tenant's plan.
    /// </summary>
    /// <param name="featureName">The feature name to check (e.g., "AdvancedReporting").</param>
    bool IsEnabled(string featureName);

    /// <summary>
    /// Returns all features enabled for the current tenant's plan.
    /// </summary>
    IReadOnlySet<string> GetEnabledFeatures();
}
