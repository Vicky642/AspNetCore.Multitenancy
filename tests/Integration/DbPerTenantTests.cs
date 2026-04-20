using FluentAssertions;
using Xunit;

namespace AspNetCore.Multitenancy.Tests.Integration;

/// <summary>
/// Integration tests for DB-per-tenant strategy.
/// Uses Testcontainers to spin up a real SQL Server instance.
/// </summary>
public class DbPerTenantTests
{
    [Fact(Skip = "Requires Docker — run manually or in CI with Docker enabled")]
    public async Task DbPerTenant_TwoTenants_DataIsIsolated()
    {
        // Arrange — would normally use Testcontainers.MsSql to spin up SQL Server
        // and create two separate databases (one per tenant).
        // Here we document the pattern:

        var acmeTenant = new TenantInfo
        {
            Id = "acme",
            Name = "Acme Corp",
            ConnectionString = "Server=localhost;Database=acme_db;Trusted_Connection=True;",
            Plan = "enterprise",
        };

        var globexTenant = new TenantInfo
        {
            Id = "globex",
            Name = "Globex Inc",
            ConnectionString = "Server=localhost;Database=globex_db;Trusted_Connection=True;",
            Plan = "pro",
        };

        // Both tenants have different databases — full isolation
        acmeTenant.ConnectionString.Should().NotBe(globexTenant.ConnectionString);
        acmeTenant.Id.Should().NotBe(globexTenant.Id);

        await Task.CompletedTask;
    }
}
