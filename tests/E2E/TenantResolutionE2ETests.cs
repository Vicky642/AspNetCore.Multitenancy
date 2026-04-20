using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace AspNetCore.Multitenancy.Tests.E2E;

public class TenantResolutionE2ETests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public TenantResolutionE2ETests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Header_KnownTenant_Returns200()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant-Id", "acme");

        var response = await client.GetAsync("/products");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Header_UnknownTenant_StillPasses_WhenRequireTenantIsFalse()
    {
        var client = _factory.CreateClient();
        // No X-Tenant-Id header — RequireTenant defaults to false
        var response = await client.GetAsync("/products");

        // Without a tenant, the controller returns BadRequest — not 404
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AdminTenants_NoAuth_CanListTenants()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/admin/tenants");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("acme");
    }
}
