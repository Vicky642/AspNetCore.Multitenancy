using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AspNetCore.Multitenancy.EntityFrameworkCore;

/// <summary>
/// Creates <typeparamref name="TContext"/> instances configured for a specific tenant.
/// Used by the DB-per-tenant strategy to swap connection strings at runtime.
/// </summary>
/// <typeparam name="TContext">Your application's DbContext type.</typeparam>
public class TenantDbContextFactory<TContext> where TContext : DbContext
{
    private readonly ITenantContext _tenantContext;
    private readonly IServiceProvider _serviceProvider;
    private readonly Action<DbContextOptionsBuilder, string> _configureOptions;

    /// <param name="tenantContext">The active tenant context.</param>
    /// <param name="serviceProvider">The DI service provider.</param>
    /// <param name="configureOptions">
    /// A delegate that receives the options builder and the tenant's connection string,
    /// and configures the database provider. Example:
    /// <code>
    /// (opts, cs) => opts.UseSqlServer(cs)
    /// </code>
    /// </param>
    public TenantDbContextFactory(
        ITenantContext tenantContext,
        IServiceProvider serviceProvider,
        Action<DbContextOptionsBuilder, string> configureOptions)
    {
        _tenantContext = tenantContext;
        _serviceProvider = serviceProvider;
        _configureOptions = configureOptions;
    }

    /// <summary>
    /// Creates a <typeparamref name="TContext"/> using the current tenant's connection string.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the current tenant has no <see cref="TenantInfo.ConnectionString"/>.
    /// </exception>
    public TContext CreateContext()
    {
        var connectionString = _tenantContext.CurrentTenant.ConnectionString
            ?? throw new InvalidOperationException(
                $"Tenant '{_tenantContext.CurrentTenant.Id}' has no ConnectionString set. " +
                "ConnectionString is required for the DB-per-tenant isolation strategy.");

        var optionsBuilder = new DbContextOptionsBuilder<TContext>();
        _configureOptions(optionsBuilder, connectionString);

        return (TContext)Activator.CreateInstance(typeof(TContext), optionsBuilder.Options, _tenantContext)!;
    }
}
