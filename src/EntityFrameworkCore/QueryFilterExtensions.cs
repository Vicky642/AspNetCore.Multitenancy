using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AspNetCore.Multitenancy.EntityFrameworkCore;

/// <summary>
/// EF Core extension methods for applying tenant query filters to entities
/// that implement <see cref="ITenantEntity"/>.
/// </summary>
public static class QueryFilterExtensions
{
    /// <summary>
    /// Applies a global query filter so that all queries for <typeparamref name="TEntity"/>
    /// are automatically scoped to the current tenant.
    /// Call inside <c>OnModelCreating</c>.
    /// </summary>
    /// <typeparam name="TEntity">Entity type implementing <see cref="ITenantEntity"/>.</typeparam>
    /// <param name="builder">The entity type builder.</param>
    /// <param name="currentTenantId">A delegate that returns the current tenant ID at query time.</param>
    public static EntityTypeBuilder<TEntity> HasTenantFilter<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        Func<string?> currentTenantId)
        where TEntity : class, ITenantEntity
    {
        builder.HasQueryFilter(e => e.TenantId == currentTenantId());
        return builder;
    }

    /// <summary>
    /// Applies tenant query filters to ALL entity types in the model that implement
    /// <see cref="ITenantEntity"/>. Call inside <c>OnModelCreating</c>.
    /// </summary>
    public static ModelBuilder ApplyTenantQueryFilters(this ModelBuilder modelBuilder, Func<string?> currentTenantId)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
                continue;

            var method = typeof(QueryFilterExtensions)
                .GetMethod(nameof(ApplyFilterToEntity), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                .MakeGenericMethod(entityType.ClrType);

            method.Invoke(null, [modelBuilder, currentTenantId]);
        }

        return modelBuilder;
    }

    private static void ApplyFilterToEntity<TEntity>(ModelBuilder builder, Func<string?> tenantId)
        where TEntity : class, ITenantEntity
    {
        builder.Entity<TEntity>().HasQueryFilter(e => e.TenantId == tenantId());
    }
}
