using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Multitenancy.EntityFrameworkCore;

/// <summary>
/// Extension methods on <see cref="MultitenancyBuilder"/> for configuring EF Core tenant isolation.
/// </summary>
public static class EFCoreExtensions
{
    /// <summary>
    /// Registers the EF Core isolation services for the specified DbContext.
    /// This sets up the <see cref="TenantDbContextFactory{TContext}"/> and
    /// <see cref="TenantSchemaMigrator{TContext}"/>.
    /// </summary>
    /// <typeparam name="TContext">Your application DbContext.</typeparam>
    /// <param name="builder">The multitenancy builder.</param>
    /// <param name="configureDb">
    /// A delegate that applies the database provider to the context options.
    /// Example: <c>(opts, cs) => opts.UseSqlServer(cs)</c>
    /// </param>
    public static MultitenancyBuilder WithEFCoreIsolation<TContext>(
        this MultitenancyBuilder builder,
        Action<DbContextOptionsBuilder, string>? configureDb = null)
        where TContext : DbContext
    {
        configureDb ??= (opts, cs) => opts.UseSqlServer(cs);

        builder.Services.AddScoped(sp =>
            new TenantDbContextFactory<TContext>(
                sp.GetRequiredService<ITenantContext>(),
                sp,
                configureDb));

        builder.Services.AddScoped<TenantSchemaMigrator<TContext>>();

        return builder;
    }
}
