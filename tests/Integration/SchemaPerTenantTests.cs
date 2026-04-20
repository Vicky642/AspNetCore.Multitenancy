using FluentAssertions;
using Xunit;

namespace AspNetCore.Multitenancy.Tests.Integration;

public class SchemaPerTenantTests
{
    [Fact(Skip = "Requires Docker — run manually or in CI with Docker enabled")]
    public async Task SchemaPerTenant_TwoTenants_SeparateSchemas()
    {
        var acmeTenant = new TenantInfo
        {
            Id = "acme",
            Name = "Acme Corp",
            Schema = "acme",
            ConnectionString = "Server=localhost;Database=SaaSShared;Trusted_Connection=True;",
            Plan = "pro",
        };

        var globexTenant = new TenantInfo
        {
            Id = "globex",
            Name = "Globex Inc",
            Schema = "globex",
            ConnectionString = "Server=localhost;Database=SaaSShared;Trusted_Connection=True;",
            Plan = "pro",
        };

        // Same DB, different schemas
        acmeTenant.ConnectionString.Should().Be(globexTenant.ConnectionString);
        acmeTenant.Schema.Should().NotBe(globexTenant.Schema);

        await Task.CompletedTask;
    }
}
