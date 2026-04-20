namespace AspNetCore.Multitenancy;

/// <summary>
/// Provides access to the current tenant context for the active request or execution scope.
/// Inject this interface anywhere you need to know which tenant is in context.
/// </summary>
public interface ITenantContext
{
    /// <summary>
    /// Gets the current tenant's information.
    /// </summary>
    /// <exception cref="TenantNotFoundException">Thrown if accessed when <see cref="HasTenant"/> is <c>false</c>.</exception>
    TenantInfo CurrentTenant { get; }

    /// <summary>
    /// Gets a value indicating whether a tenant has been resolved for the current context.
    /// </summary>
    bool HasTenant { get; }

    /// <summary>
    /// Creates an isolated DI scope for the specified tenant, setting ITenantContext
    /// to that tenant within the scope. Useful for background jobs.
    /// </summary>
    /// <param name="tenantId">The tenant identifier to scope to.</param>
    IServiceScope CreateTenantScope(string tenantId);
}
