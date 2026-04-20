namespace AspNetCore.Multitenancy.Resolvers;

/// <summary>
/// Resolves the current tenant from the request's host/subdomain.
/// Example: <c>acme.app.com</c> resolves tenant ID <c>acme</c>.
/// </summary>
public class HostTenantResolver : ITenantResolver
{
    private readonly ITenantStore _store;
    private readonly string? _baseDomain;

    /// <param name="store">The tenant store to look up tenants.</param>
    /// <param name="baseDomain">
    /// Optional. The base domain to strip (e.g., "app.com").
    /// If provided, "acme.app.com" → "acme".
    /// If null, uses the first label of the host as tenant ID.
    /// </param>
    public HostTenantResolver(ITenantStore store, string? baseDomain = null)
    {
        _store = store;
        _baseDomain = baseDomain?.TrimStart('.');
    }

    /// <inheritdoc />
    public async Task<TenantInfo?> ResolveAsync(HttpContext context)
    {
        var host = context.Request.Host.Host;
        if (string.IsNullOrWhiteSpace(host))
            return null;

        string tenantId;

        if (_baseDomain is not null && host.EndsWith(_baseDomain, StringComparison.OrdinalIgnoreCase))
        {
            if (host.Length == _baseDomain.Length)
                return null; // Exact match, no subdomain

            // Strip ".app.com" to get "acme"
            tenantId = host[..^(_baseDomain.Length + 1)]; // +1 for the dot
        }
        else
        {
            // Take the first subdomain label
            var dotIndex = host.IndexOf('.');
            tenantId = dotIndex > 0 ? host[..dotIndex] : host;
        }

        if (string.IsNullOrWhiteSpace(tenantId))
            return null;

        var tenant = await _store.GetTenantAsync(tenantId);
        return tenant?.IsActive == true ? tenant : null;
    }
}
