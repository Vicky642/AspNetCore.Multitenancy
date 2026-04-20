using AspNetCore.Multitenancy;
using AspNetCore.Multitenancy.FeatureFlags;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SaaSDemo.Products;

/// <summary>
/// Tenant-scoped Products API.
/// All queries are automatically filtered to the current tenant via EF Core global query filters.
/// The [RequireFeature] attribute on the reports endpoint demonstrates plan gating.
/// </summary>
[ApiController]
[Route("products")]
public class ProductsController : ControllerBase
{
    private readonly ProductsDbContext _db;
    private readonly ITenantContext _tenantContext;

    public ProductsController(ProductsDbContext db, ITenantContext tenantContext)
    {
        _db = db;
        _tenantContext = tenantContext;
    }

    /// <summary>List products for the current tenant.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (!_tenantContext.HasTenant)
            return BadRequest("No tenant resolved. Provide the X-Tenant-Id header.");

        // EF Core global query filter automatically scopes to current tenant
        var products = await _db.Products.Where(p => p.IsActive).ToListAsync();
        return Ok(products);
    }

    /// <summary>Get a single product.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        // Global filter already scopes to current tenant — no manual TenantId check needed
        var product = await _db.Products.FindAsync(id);
        return product is null ? NotFound() : Ok(product);
    }

    /// <summary>Create a product for the current tenant.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
    {
        if (!_tenantContext.HasTenant)
            return BadRequest("No tenant resolved.");

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description ?? string.Empty,
            Price = request.Price,
            // TenantId is stamped automatically by MultitenantDbContext.SaveChanges
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
    }

    /// <summary>
    /// Advanced product report — requires the AdvancedReporting feature (Pro plan and above).
    /// Free-tier tenants receive HTTP 404 (not 403) so they cannot enumerate unavailable features.
    /// </summary>
    [HttpGet("reports/advanced")]
    [RequireFeature("AdvancedReporting")]
    public async Task<IActionResult> AdvancedReport()
    {
        var summary = await _db.Products
            .GroupBy(p => p.IsActive)
            .Select(g => new { IsActive = g.Key, Total = g.Count(), Revenue = g.Sum(p => p.Price) })
            .ToListAsync();

        return Ok(new
        {
            Tenant = _tenantContext.CurrentTenant.Id,
            GeneratedAt = DateTimeOffset.UtcNow,
            Summary = summary,
        });
    }

    /// <summary>Delete a product.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return NotFound();

        product.IsActive = false;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

public record CreateProductRequest(string Name, string? Description, decimal Price);
