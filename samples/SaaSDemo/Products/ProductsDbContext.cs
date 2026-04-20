using AspNetCore.Multitenancy;
using AspNetCore.Multitenancy.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SaaSDemo.Products;

/// <summary>
/// Application DbContext that inherits from <see cref="MultitenantDbContext"/>.
/// Tenant query filters and TenantId stamping are handled by the base class.
/// </summary>
public class ProductsDbContext : MultitenantDbContext
{
    public ProductsDbContext(DbContextOptions<ProductsDbContext> options, ITenantContext tenantContext)
        : base(options, tenantContext)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // applies tenant query filters

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Price).HasPrecision(18, 2);
            entity.Property(p => p.TenantId).IsRequired().HasMaxLength(100);
        });
    }
}
