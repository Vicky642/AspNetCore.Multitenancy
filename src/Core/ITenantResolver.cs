namespace AspNetCore.Multitenancy;

/// <summary>
/// Resolves the current tenant from an incoming HTTP request.
/// Implement this interface to create a custom resolution strategy.
/// Multiple resolvers can be registered; they are tried in order until one succeeds.
/// </summary>
public interface ITenantResolver
{
    /// <summary>
    /// Attempts to resolve a <see cref="TenantInfo"/> from the given HTTP context.
    /// Returns <c>null</c> if this resolver cannot determine the tenant.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>The resolved tenant, or <c>null</c> if not resolvable by this strategy.</returns>
    Task<TenantInfo?> ResolveAsync(HttpContext context);
}
