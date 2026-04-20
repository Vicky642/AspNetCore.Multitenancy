using System.Security.Claims;

namespace AspNetCore.Multitenancy.Resolvers;

/// <summary>
/// Resolves the current tenant from a JWT claim.
/// Default claim type: <c>tid</c> (tenant ID).
/// Requires the request to be authenticated.
/// </summary>
public class ClaimsTenantResolver : ITenantResolver
{
    private readonly ITenantStore _store;
    private readonly string _claimType;

    /// <param name="store">The tenant store to look up the resolved ID.</param>
    /// <param name="claimType">The claim type that carries the tenant ID. Defaults to <c>tid</c>.</param>
    public ClaimsTenantResolver(ITenantStore store, string claimType = "tid")
    {
        _store = store;
        _claimType = claimType;
    }

    /// <inheritdoc />
    public async Task<TenantInfo?> ResolveAsync(HttpContext context)
    {
        var user = context.User;
        if (user?.Identity?.IsAuthenticated != true)
            return null;

        var tenantId = user.FindFirstValue(_claimType);
        if (string.IsNullOrWhiteSpace(tenantId))
            return null;

        var tenant = await _store.GetTenantAsync(tenantId);
        return tenant?.IsActive == true ? tenant : null;
    }
}
