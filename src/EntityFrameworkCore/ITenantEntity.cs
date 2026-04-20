namespace AspNetCore.Multitenancy.EntityFrameworkCore;

/// <summary>
/// Marks an entity as tenant-scoped. The EF Core model builder and
/// <see cref="MultitenantDbContext"/> use this interface to apply global query
/// filters and to stamp <see cref="TenantId"/> on insert automatically.
/// </summary>
public interface ITenantEntity
{
    /// <summary>
    /// The tenant that owns this entity record.
    /// Set automatically on SaveChanges — do not set manually.
    /// </summary>
    string TenantId { get; set; }
}
