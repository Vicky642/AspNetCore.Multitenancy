using System.Collections.Concurrent;

namespace AspNetCore.Multitenancy;

/// <summary>
/// Simple in-memory implementation of <see cref="ITenantStore"/>.
/// Use for unit tests, demos, and local development.
/// Not suitable for production multi-instance deployments.
/// </summary>
public class InMemoryTenantStore : ITenantStore
{
    private readonly ConcurrentDictionary<string, TenantInfo> _tenants = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes the store with an optional seed of tenants.
    /// </summary>
    public InMemoryTenantStore(IEnumerable<TenantInfo>? seed = null)
    {
        if (seed is not null)
        {
            foreach (var t in seed)
                _tenants[t.Id] = t;
        }
    }

    /// <inheritdoc />
    public Task<TenantInfo?> GetTenantAsync(string tenantId, CancellationToken cancellationToken = default)
        => Task.FromResult(_tenants.TryGetValue(tenantId, out var t) ? t : null);

    /// <inheritdoc />
    public Task<IReadOnlyList<TenantInfo>> GetAllTenantsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<TenantInfo>>(_tenants.Values.ToList());

    /// <inheritdoc />
    public Task AddTenantAsync(TenantInfo tenant, CancellationToken cancellationToken = default)
    {
        _tenants[tenant.Id] = tenant;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task UpdateTenantAsync(TenantInfo tenant, CancellationToken cancellationToken = default)
    {
        _tenants[tenant.Id] = tenant;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RemoveTenantAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        _tenants.TryRemove(tenantId, out _);
        return Task.CompletedTask;
    }
}
