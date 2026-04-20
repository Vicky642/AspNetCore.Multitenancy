using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Multitenancy.EntityFrameworkCore.Strategies;

/// <summary>
/// Isolation strategy: each tenant has a completely separate SQL Server/PostgreSQL database.
/// Maximum isolation. Recommended for enterprise and regulated tenants.
///
/// Usage in Program.cs:
/// <code>
/// builder.Services.AddDbContext&lt;AppDbContext&gt;((sp, opts) =>
///     DbPerTenantStrategy.Configure(opts, sp));
/// </code>
/// </summary>
public static class DbPerTenantStrategy
{
    /// <summary>
    /// Configures a <see cref="DbContextOptionsBuilder"/> to use the current tenant's
    /// <see cref="TenantInfo.ConnectionString"/> at runtime.
    /// </summary>
    /// <param name="optionsBuilder">The EF Core options builder to configure.</param>
    /// <param name="serviceProvider">DI provider used to resolve <see cref="ITenantContext"/>.</param>
    /// <param name="useProvider">
    /// A callback that applies the database provider, e.g.:
    /// <c>(opts, cs) => opts.UseSqlServer(cs)</c>
    /// </param>
    public static void Configure(
        DbContextOptionsBuilder optionsBuilder,
        IServiceProvider serviceProvider,
        Action<DbContextOptionsBuilder, string>? useProvider = null)
    {
        var tenantContext = serviceProvider.GetService<ITenantContext>();
        if (tenantContext?.HasTenant == true)
        {
            var connectionString = tenantContext.CurrentTenant.ConnectionString;
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                useProvider?.Invoke(optionsBuilder, connectionString);
            }
        }
    }
}
