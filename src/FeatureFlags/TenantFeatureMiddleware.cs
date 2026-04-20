using Microsoft.AspNetCore.Routing;

namespace AspNetCore.Multitenancy.FeatureFlags;

/// <summary>
/// Middleware that checks <see cref="RequireFeatureAttribute"/> on the matched endpoint
/// and returns HTTP 404 if the feature is disabled for the current tenant's plan.
///
/// Returns 404 (not 403) deliberately — so tenants cannot enumerate which features exist
/// on other plans.
/// </summary>
public class TenantFeatureMiddleware
{
    private readonly RequestDelegate _next;

    public TenantFeatureMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantFeatureService featureService)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint is not null)
        {
            var featureAttr = endpoint.Metadata.GetMetadata<RequireFeatureAttribute>();
            if (featureAttr is not null && !featureService.IsEnabled(featureAttr.FeatureName))
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }
        }

        await _next(context);
    }
}
