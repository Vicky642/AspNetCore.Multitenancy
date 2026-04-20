using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Multitenancy.EntityFrameworkCore.Strategies;

/// <summary>
/// Isolation strategy: all tenants share one database but each gets a dedicated schema
/// (e.g., <c>acme.Products</c>, <c>globex.Products</c>).
/// Good balance of isolation vs cost.
///
/// The schema is taken from <see cref="TenantInfo.Schema"/>. If not set, falls back to
/// <see cref="TenantInfo.Id"/>.
/// </summary>
public static class SchemaPerTenantStrategy
{
    /// <summary>
    /// Applies the current tenant's schema as the EF Core default schema.
    /// Call from <c>OnModelCreating</c>:
    /// <code>
    /// protected override void OnModelCreating(ModelBuilder builder)
    /// {
    ///     SchemaPerTenantStrategy.Apply(builder, _tenantContext);
    ///     base.OnModelCreating(builder);
    /// }
    /// </code>
    /// </summary>
    public static void Apply(ModelBuilder modelBuilder, ITenantContext tenantContext)
    {
        if (!tenantContext.HasTenant)
            return;

        var tenant = tenantContext.CurrentTenant;
        var schema = !string.IsNullOrWhiteSpace(tenant.Schema) ? tenant.Schema : tenant.Id;
        modelBuilder.HasDefaultSchema(schema);
    }
}
