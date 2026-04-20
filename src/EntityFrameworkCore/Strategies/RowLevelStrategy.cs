using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Multitenancy.EntityFrameworkCore.Strategies;

/// <summary>
/// Isolation strategy: all tenants share tables. Every table has a <c>TenantId</c> column.
/// EF Core global query filters automatically scope all queries.
/// Cheapest to operate. Recommended for low-risk SaaS.
///
/// This strategy is applied automatically by <see cref="MultitenantDbContext"/>.
/// Entities must implement <see cref="ITenantEntity"/>.
/// </summary>
public static class RowLevelStrategy
{
    /// <summary>
    /// Configures the model for row-level tenant isolation.
    /// Applies <see cref="QueryFilterExtensions.ApplyTenantQueryFilters"/> to all
    /// <see cref="ITenantEntity"/> entity types and adds a TenantId index for performance.
    /// </summary>
    public static void Apply(ModelBuilder modelBuilder, Func<string?> getCurrentTenantId)
    {
        modelBuilder.ApplyTenantQueryFilters(getCurrentTenantId);

        // Add an index on TenantId for every tenant-scoped entity for query performance
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
                continue;

            modelBuilder.Entity(entityType.ClrType)
                .HasIndex("TenantId")
                .HasDatabaseName($"IX_{entityType.GetTableName()}_TenantId");
        }
    }
}
