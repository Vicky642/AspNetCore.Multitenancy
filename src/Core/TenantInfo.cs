namespace AspNetCore.Multitenancy;

/// <summary>
/// Represents a tenant in the system. All properties needed for routing,
/// database isolation, and feature access live here.
/// </summary>
public class TenantInfo
{
    /// <summary>
    /// Unique slug identifier for the tenant (e.g., "acme").
    /// Used as the key for resolution and path prefixing.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable display name (e.g., "Acme Corp").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Connection string for DB-per-tenant isolation strategy.
    /// Leave empty for schema-per-tenant or row-level strategies.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Database schema name for schema-per-tenant isolation (e.g., "acme").
    /// Leave empty for DB-per-tenant or row-level strategies.
    /// </summary>
    public string? Schema { get; set; }

    /// <summary>
    /// Subscription plan identifier. Used for feature flag gating.
    /// Typical values: "free", "pro", "enterprise".
    /// </summary>
    public string Plan { get; set; } = "free";

    /// <summary>
    /// Arbitrary key-value settings for tenant-specific configuration.
    /// </summary>
    public Dictionary<string, string> Settings { get; set; } = new();

    /// <summary>
    /// Whether this tenant is active. Inactive tenants are rejected during resolution.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// When the tenant was created (UTC).
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
