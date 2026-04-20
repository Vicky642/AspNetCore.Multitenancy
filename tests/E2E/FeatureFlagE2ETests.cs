using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace AspNetCore.Multitenancy.Tests.E2E;

public class FeatureFlagE2ETests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public FeatureFlagE2ETests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AdvancedReport_EnterpriseTenant_Returns200()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant-Id", "acme"); // enterprise plan

        var response = await client.GetAsync("/products/reports/advanced");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AdvancedReport_FreeTenant_Returns404()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant-Id", "initech"); // free plan

        var response = await client.GetAsync("/products/reports/advanced");

        // 404 (not 403) — tenants cannot enumerate disabled features
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AdvancedReport_ProTenant_Returns200()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant-Id", "globex"); // pro plan

        var response = await client.GetAsync("/products/reports/advanced");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
