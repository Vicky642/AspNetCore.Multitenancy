namespace AspNetCore.Multitenancy;

/// <summary>
/// Specifies where tenants are persisted.
/// </summary>
public enum TenantStore
{
    /// <summary>In-memory store — for development and testing only.</summary>
    InMemory,

    /// <summary>Tenants are stored in a database via Entity Framework Core.</summary>
    EntityFramework,

    /// <summary>Tenants are loaded from appsettings.json configuration.</summary>
    Configuration,
}

/// <summary>
/// Specifies the database isolation strategy for tenant data.
/// </summary>
public enum IsolationStrategy
{
    /// <summary>Each tenant gets a completely separate database.</summary>
    DbPerTenant,

    /// <summary>All tenants share a database but use separate schemas.</summary>
    SchemaPerTenant,

    /// <summary>All tenants share tables; data is filtered by a TenantId column.</summary>
    RowLevel,
}

/// <summary>
/// Top-level configuration options for the multi-tenancy library.
/// Configure via the <c>AddMultitenancy(options => ...)</c> builder.
/// </summary>
public class MultitenancyOptions
{
    /// <summary>
    /// Gets or sets where tenant records are persisted.
    /// Default: <see cref="TenantStore.InMemory"/>.
    /// </summary>
    public TenantStore TenantStore { get; set; } = TenantStore.InMemory;

    /// <summary>
    /// Gets or sets the database isolation strategy.
    /// Default: <see cref="IsolationStrategy.RowLevel"/>.
    /// </summary>
    public IsolationStrategy IsolationStrategy { get; set; } = IsolationStrategy.RowLevel;

    /// <summary>
    /// When <c>true</c>, a request with no resolvable tenant returns HTTP 404.
    /// When <c>false</c>, the request proceeds with <see cref="ITenantContext.HasTenant"/> == false.
    /// Default: <c>false</c>.
    /// </summary>
    public bool RequireTenant { get; set; } = false;

    /// <summary>
    /// HTTP status code returned when tenant resolution fails and <see cref="RequireTenant"/> is <c>true</c>.
    /// Default: 404.
    /// </summary>
    public int TenantNotFoundStatusCode { get; set; } = 404;
}
