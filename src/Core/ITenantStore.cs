namespace AspNetCore.Multitenancy;

/// <summary>
/// Provides persistent storage and retrieval of <see cref="TenantInfo"/> records.
/// Implement this to back tenants with a database, configuration file, or any other source.
/// </summary>
public interface ITenantStore
{
    /// <summary>
    /// Retrieves a tenant by its unique identifier.
    /// Returns <c>null</c> if not found.
    /// </summary>
    Task<TenantInfo?> GetTenantAsync(string tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all tenants known to the store.
    /// </summary>
    Task<IReadOnlyList<TenantInfo>> GetAllTenantsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new tenant to the store.
    /// </summary>
    Task AddTenantAsync(TenantInfo tenant, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing tenant in the store.
    /// </summary>
    Task UpdateTenantAsync(TenantInfo tenant, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a tenant from the store.
    /// </summary>
    Task RemoveTenantAsync(string tenantId, CancellationToken cancellationToken = default);
}
