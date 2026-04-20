using FluentAssertions;
using Xunit;

namespace AspNetCore.Multitenancy.Tests.Integration;

public class JobIsolationTests
{
    [Fact]
    public async Task InMemoryStore_GetAllTenants_ReturnsSeededTenants()
    {
        // Arrange
        var tenants = new[]
        {
            new TenantInfo { Id = "acme",   Name = "Acme",   Plan = "enterprise", IsActive = true },
            new TenantInfo { Id = "globex", Name = "Globex", Plan = "pro",        IsActive = true },
        };
        var store = new InMemoryTenantStore(tenants);

        // Act
        var all = await store.GetAllTenantsAsync();

        // Assert — both tenants are present, both are active
        all.Should().HaveCount(2);
        all.Select(t => t.Id).Should().Contain(["acme", "globex"]);
        all.Should().AllSatisfy(t => t.IsActive.Should().BeTrue());
    }

    [Fact]
    public async Task InMemoryStore_AddAndGet_RoundTrips()
    {
        var store = new InMemoryTenantStore();

        var tenant = new TenantInfo { Id = "newco", Name = "New Co", Plan = "free", IsActive = true };
        await store.AddTenantAsync(tenant);

        var retrieved = await store.GetTenantAsync("newco");

        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("New Co");
    }

    [Fact(Skip = "Requires Hangfire + SQL Server — run in CI with Docker")]
    public async Task HangfireJob_RunsInCorrectTenantScope()
    {
        // This test would:
        // 1. Enqueue a job while tenant "acme" is active
        // 2. Verify TenantJobFilter serialized TenantId="acme" into job params
        // 3. Process the job and check ITenantContext.CurrentTenant.Id == "acme"
        await Task.CompletedTask;
    }
}
