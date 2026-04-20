namespace AspNetCore.Multitenancy;

/// <summary>
/// Default implementation of <see cref="ITenantContext"/>.
/// Registered as a scoped service so it is unique per HTTP request.
/// </summary>
internal sealed class TenantContext : ITenantContext
{
    private TenantInfo? _currentTenant;
    private readonly IServiceProvider _serviceProvider;

    public TenantContext(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public TenantInfo CurrentTenant
        => _currentTenant ?? throw new TenantNotFoundException();

    /// <inheritdoc />
    public bool HasTenant => _currentTenant is not null;

    /// <summary>
    /// Sets the current tenant. Called by <see cref="TenantMiddleware"/> after resolution.
    /// </summary>
    internal void SetTenant(TenantInfo tenant) => _currentTenant = tenant;

    /// <inheritdoc />
    public IServiceScope CreateTenantScope(string tenantId)
    {
        var store = _serviceProvider.GetRequiredService<ITenantStore>();
        var tenant = store.GetTenantAsync(tenantId).GetAwaiter().GetResult()
            ?? throw new TenantNotFoundException(tenantId);

        var scope = _serviceProvider.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<ITenantContext>() as TenantContext;
        ctx?.SetTenant(tenant);
        return scope;
    }
}
