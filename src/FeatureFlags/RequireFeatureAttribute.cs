namespace AspNetCore.Multitenancy.FeatureFlags;

/// <summary>
/// Marks a controller or action as requiring a specific tenant feature.
/// If the feature is disabled for the current tenant's plan, the middleware
/// returns HTTP 404 (not 403) so that other tenants cannot enumerate which
/// features exist.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class RequireFeatureAttribute : Attribute
{
    /// <summary>
    /// Gets the feature name required to access the decorated endpoint.
    /// </summary>
    public string FeatureName { get; }

    /// <param name="featureName">The feature name (e.g., "AdvancedReporting").</param>
    public RequireFeatureAttribute(string featureName)
    {
        FeatureName = featureName;
    }
}
