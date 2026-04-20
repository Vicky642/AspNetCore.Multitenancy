namespace AspNetCore.Multitenancy.FileStorage;

/// <summary>
/// Prepends the current tenant ID to all storage paths, ensuring complete
/// isolation between tenants even in a shared storage backend.
///
/// Example: tenant "acme" + path "invoices/jan.pdf" → "tenant-acme/invoices/jan.pdf"
/// </summary>
public class TenantStoragePathBuilder
{
    private readonly ITenantContext _tenantContext;

    public TenantStoragePathBuilder(ITenantContext tenantContext)
    {
        _tenantContext = tenantContext;
    }

    /// <summary>
    /// Builds the fully-qualified storage path for the current tenant.
    /// </summary>
    /// <param name="path">The logical path (e.g., "invoices/jan.pdf").</param>
    /// <returns>The tenant-prefixed path (e.g., "tenant-acme/invoices/jan.pdf").</returns>
    public string Build(string path)
    {
        if (!_tenantContext.HasTenant)
            throw new InvalidOperationException("Cannot build a storage path: no tenant is in context.");

        var tenantId = _tenantContext.CurrentTenant.Id;
        var cleanPath = path.TrimStart('/');
        return $"tenant-{tenantId}/{cleanPath}";
    }

    /// <summary>
    /// Strips the tenant prefix from a full storage path, returning the logical path.
    /// </summary>
    public string StripPrefix(string fullPath)
    {
        if (!_tenantContext.HasTenant)
            return fullPath;

        var prefix = $"tenant-{_tenantContext.CurrentTenant.Id}/";
        return fullPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
            ? fullPath[prefix.Length..]
            : fullPath;
    }
}
