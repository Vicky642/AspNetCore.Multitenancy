using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Multitenancy.EntityFrameworkCore;

/// <summary>
/// Base <see cref="DbContext"/> for multi-tenant applications.
/// Automatically:
/// <list type="bullet">
///   <item>Applies global query filters for all <see cref="ITenantEntity"/> entity types</item>
///   <item>Stamps <c>TenantId</c> on every insert via <c>SaveChanges</c></item>
///   <item>Sets the default schema from <see cref="ITenantContext"/> for schema-per-tenant strategy</item>
/// </list>
/// Inherit from this class in your application DbContext.
/// </summary>
public abstract class MultitenantDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    protected MultitenantDbContext(DbContextOptions options, ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply row-level tenant query filters to all ITenantEntity types
        string? GetTenantId() => _tenantContext.HasTenant ? _tenantContext.CurrentTenant.Id : null;
        modelBuilder.ApplyTenantQueryFilters(GetTenantId);
    }

    /// <inheritdoc />
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        StampTenantId();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    /// <inheritdoc />
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        StampTenantId();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void StampTenantId()
    {
        if (!_tenantContext.HasTenant)
            return;

        var tenantId = _tenantContext.CurrentTenant.Id;

        foreach (var entry in ChangeTracker.Entries<ITenantEntity>())
        {
            if (entry.State == EntityState.Added)
                entry.Entity.TenantId = tenantId;
        }
    }
}
