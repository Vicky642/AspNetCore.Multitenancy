namespace AspNetCore.Multitenancy.Resolvers;

/// <summary>
/// Resolves the current tenant from the URL path prefix.
/// Example: <c>/acme/products</c> resolves tenant ID <c>acme</c> and rewrites path to <c>/products</c>.
/// </summary>
public class PathTenantResolver : ITenantResolver
{
    private readonly ITenantStore _store;

    public PathTenantResolver(ITenantStore store)
    {
        _store = store;
    }

    /// <inheritdoc />
    public async Task<TenantInfo?> ResolveAsync(HttpContext context)
    {
        var path = context.Request.Path.Value;
        if (string.IsNullOrWhiteSpace(path) || path.Length < 2)
            return null;

        // Path format: /{tenantId}/...
        var segments = path.TrimStart('/').Split('/', 2);
        var tenantId = segments[0];

        if (string.IsNullOrWhiteSpace(tenantId))
            return null;

        var tenant = await _store.GetTenantAsync(tenantId);
        if (tenant?.IsActive != true)
            return null;

        // Rewrite path: remove the tenant prefix so downstream routing works normally
        var remainingPath = segments.Length > 1 ? "/" + segments[1] : "/";
        context.Request.Path = new PathString(remainingPath);

        return tenant;
    }
}
