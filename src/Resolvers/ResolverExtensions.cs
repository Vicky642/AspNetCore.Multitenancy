namespace AspNetCore.Multitenancy.Resolvers;

/// <summary>
/// Extension methods on <see cref="MultitenancyBuilder"/> for registering resolver strategies.
/// </summary>
public static class ResolverExtensions
{
    /// <summary>
    /// Adds a <see cref="HeaderTenantResolver"/> that reads tenant ID from an HTTP header.
    /// </summary>
    /// <param name="builder">The multitenancy builder.</param>
    /// <param name="headerName">The header to read. Defaults to <c>X-Tenant-Id</c>.</param>
    public static MultitenancyBuilder WithHeaderResolver(this MultitenancyBuilder builder, string headerName = "X-Tenant-Id")
    {
        builder.Services.AddScoped<ITenantResolver>(sp =>
            new HeaderTenantResolver(sp.GetRequiredService<ITenantStore>(), headerName));
        return builder;
    }

    /// <summary>
    /// Adds a <see cref="HostTenantResolver"/> that resolves tenant from the request subdomain.
    /// </summary>
    /// <param name="builder">The multitenancy builder.</param>
    /// <param name="baseDomain">Optional base domain to strip (e.g., "app.com").</param>
    public static MultitenancyBuilder WithHostResolver(this MultitenancyBuilder builder, string? baseDomain = null)
    {
        builder.Services.AddScoped<ITenantResolver>(sp =>
            new HostTenantResolver(sp.GetRequiredService<ITenantStore>(), baseDomain));
        return builder;
    }

    /// <summary>
    /// Adds a <see cref="ClaimsTenantResolver"/> that resolves tenant from a JWT claim.
    /// </summary>
    /// <param name="builder">The multitenancy builder.</param>
    /// <param name="claimType">The claim type that carries the tenant ID. Defaults to <c>tid</c>.</param>
    public static MultitenancyBuilder WithClaimsResolver(this MultitenancyBuilder builder, string claimType = "tid")
    {
        builder.Services.AddScoped<ITenantResolver>(sp =>
            new ClaimsTenantResolver(sp.GetRequiredService<ITenantStore>(), claimType));
        return builder;
    }

    /// <summary>
    /// Adds a <see cref="PathTenantResolver"/> that resolves tenant from the URL prefix.
    /// </summary>
    public static MultitenancyBuilder WithPathResolver(this MultitenancyBuilder builder)
    {
        builder.Services.AddScoped<ITenantResolver>(sp =>
            new PathTenantResolver(sp.GetRequiredService<ITenantStore>()));
        return builder;
    }
}
