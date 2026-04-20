namespace AspNetCore.Multitenancy.Resolvers;

/// <summary>
/// Resolves the current tenant from a configurable HTTP request header.
/// Default header: <c>X-Tenant-Id</c>.
/// </summary>
public class HeaderTenantResolver : ITenantResolver
{
    private readonly ITenantStore _store;
    private readonly string _headerName;

    /// <param name="store">The tenant store to look up the resolved ID against.</param>
    /// <param name="headerName">The HTTP header name to read. Defaults to <c>X-Tenant-Id</c>.</param>
    public HeaderTenantResolver(ITenantStore store, string headerName = "X-Tenant-Id")
    {
        _store = store;
        _headerName = headerName;
    }

    /// <inheritdoc />
    public async Task<TenantInfo?> ResolveAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(_headerName, out var values))
            return null;

        var tenantId = values.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(tenantId))
            return null;

        var tenant = await _store.GetTenantAsync(tenantId);
        return tenant?.IsActive == true ? tenant : null;
    }
}
