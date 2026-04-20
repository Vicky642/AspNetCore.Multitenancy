using AspNetCore.Multitenancy;
using Microsoft.AspNetCore.Mvc;
using SaaSDemo.Tenants;

namespace SaaSDemo.Tenants;

/// <summary>
/// Admin-only CRUD endpoints for tenant management.
/// In production, guard these with an admin policy or separate admin app.
/// </summary>
[ApiController]
[Route("admin/tenants")]
public class TenantsController : ControllerBase
{
    private readonly ITenantStore _store;
    private readonly TenantProvisioningService _provisioner;

    public TenantsController(ITenantStore store, TenantProvisioningService provisioner)
    {
        _store = store;
        _provisioner = provisioner;
    }

    /// <summary>List all tenants.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tenants = await _store.GetAllTenantsAsync();
        return Ok(tenants);
    }

    /// <summary>Get a single tenant by ID.</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var tenant = await _store.GetTenantAsync(id);
        return tenant is null ? NotFound() : Ok(tenant);
    }

    /// <summary>Create a new tenant and provision its resources.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTenantRequest request)
    {
        var existing = await _store.GetTenantAsync(request.Id);
        if (existing is not null)
            return Conflict($"Tenant '{request.Id}' already exists.");

        var tenant = new TenantInfo
        {
            Id = request.Id,
            Name = request.Name,
            Plan = request.Plan ?? "free",
            ConnectionString = request.ConnectionString,
            Schema = request.Schema,
            IsActive = true,
        };

        await _store.AddTenantAsync(tenant);
        await _provisioner.ProvisionAsync(tenant);

        return CreatedAtAction(nameof(Get), new { id = tenant.Id }, tenant);
    }

    /// <summary>Update an existing tenant.</summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] TenantInfo updated)
    {
        var existing = await _store.GetTenantAsync(id);
        if (existing is null)
            return NotFound();

        updated.Id = id;
        await _store.UpdateTenantAsync(updated);
        return Ok(updated);
    }

    /// <summary>Deactivate a tenant (soft delete).</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var existing = await _store.GetTenantAsync(id);
        if (existing is null)
            return NotFound();

        existing.IsActive = false;
        await _store.UpdateTenantAsync(existing);
        return NoContent();
    }
}

public record CreateTenantRequest(
    string Id,
    string Name,
    string? Plan,
    string? ConnectionString,
    string? Schema);
