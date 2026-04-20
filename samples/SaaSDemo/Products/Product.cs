using AspNetCore.Multitenancy.EntityFrameworkCore;

namespace SaaSDemo.Products;

/// <summary>
/// A sample tenant-scoped entity.
/// Each product belongs to exactly one tenant (enforced via ITenantEntity).
/// </summary>
public class Product : ITenantEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Tenant isolation — set automatically by MultitenantDbContext.SaveChanges
    public string TenantId { get; set; } = string.Empty;
}
