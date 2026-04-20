using Hangfire;
using Hangfire.Server;

namespace AspNetCore.Multitenancy.BackgroundJobs;

/// <summary>
/// Resolves Hangfire job instances from a per-tenant DI scope, ensuring that
/// any scoped services (including <see cref="ITenantContext"/>) are resolved
/// from the correct tenant scope.
/// </summary>
public class TenantJobActivator : JobActivator
{
    private readonly IServiceProvider _rootProvider;

    public TenantJobActivator(IServiceProvider rootProvider)
    {
        _rootProvider = rootProvider;
    }

    /// <inheritdoc />
    public override object ActivateJob(Type jobType)
        => _rootProvider.GetRequiredService(jobType);

    /// <inheritdoc />
    public override JobActivatorScope BeginScope(PerformContext context)
        => new TenantJobActivatorScope(_rootProvider.CreateScope());

    private sealed class TenantJobActivatorScope : JobActivatorScope
    {
        private readonly IServiceScope _scope;

        public TenantJobActivatorScope(IServiceScope scope)
        {
            _scope = scope;
        }

        public override object Resolve(Type type)
            => _scope.ServiceProvider.GetRequiredService(type);

        public override void DisposeScope()
            => _scope.Dispose();
    }
}
