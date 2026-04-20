namespace AspNetCore.Multitenancy;

/// <summary>
/// ASP.NET Core middleware that runs all registered <see cref="ITenantResolver"/>s
/// in order and populates <see cref="ITenantContext"/> for the request.
/// </summary>
public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly MultitenancyOptions _options;
    private readonly ILogger<TenantMiddleware> _logger;

    public TenantMiddleware(RequestDelegate next, MultitenancyOptions options, ILogger<TenantMiddleware> logger)
    {
        _next = next;
        _options = options;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
    {
        var resolvers = context.RequestServices.GetServices<ITenantResolver>();
        TenantInfo? resolved = null;

        foreach (var resolver in resolvers)
        {
            resolved = await resolver.ResolveAsync(context);
            if (resolved is not null)
            {
                _logger.LogDebug("Tenant '{TenantId}' resolved by {Resolver}",
                    resolved.Id, resolver.GetType().Name);
                break;
            }
        }

        if (resolved is not null && tenantContext is TenantContext mutableCtx)
        {
            mutableCtx.SetTenant(resolved);
        }
        else if (_options.RequireTenant)
        {
            _logger.LogWarning("No tenant resolved for {Path}; returning {StatusCode}",
                context.Request.Path, _options.TenantNotFoundStatusCode);
            context.Response.StatusCode = _options.TenantNotFoundStatusCode;
            return;
        }

        await _next(context);
    }
}
