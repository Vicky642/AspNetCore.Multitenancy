using AspNetCore.Multitenancy.FileStorage;
using FluentAssertions;
using Moq;
using Xunit;

namespace AspNetCore.Multitenancy.Tests.Unit;

public class StoragePathTests
{
    private static ITenantContext MakeTenantContext(string tenantId)
    {
        var mock = new Mock<ITenantContext>();
        mock.Setup(c => c.HasTenant).Returns(true);
        mock.Setup(c => c.CurrentTenant).Returns(new TenantInfo { Id = tenantId });
        return mock.Object;
    }

    [Fact]
    public void Build_PrefixesTenantId()
    {
        var ctx = MakeTenantContext("acme");
        var builder = new TenantStoragePathBuilder(ctx);

        var result = builder.Build("invoices/jan.pdf");

        result.Should().Be("tenant-acme/invoices/jan.pdf");
    }

    [Fact]
    public void Build_StripsLeadingSlash()
    {
        var ctx = MakeTenantContext("acme");
        var builder = new TenantStoragePathBuilder(ctx);

        var result = builder.Build("/invoices/jan.pdf");

        result.Should().Be("tenant-acme/invoices/jan.pdf");
    }

    [Fact]
    public void StripPrefix_RemovesTenantPrefix()
    {
        var ctx = MakeTenantContext("acme");
        var builder = new TenantStoragePathBuilder(ctx);

        var result = builder.StripPrefix("tenant-acme/invoices/jan.pdf");

        result.Should().Be("invoices/jan.pdf");
    }

    [Fact]
    public void Build_NoTenant_ThrowsInvalidOperation()
    {
        var mock = new Mock<ITenantContext>();
        mock.Setup(c => c.HasTenant).Returns(false);
        var builder = new TenantStoragePathBuilder(mock.Object);

        var act = () => builder.Build("file.pdf");

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Build_IsolateDifferentTenants()
    {
        var pathA = new TenantStoragePathBuilder(MakeTenantContext("acme")).Build("report.pdf");
        var pathB = new TenantStoragePathBuilder(MakeTenantContext("globex")).Build("report.pdf");

        pathA.Should().NotBe(pathB);
        pathA.Should().StartWith("tenant-acme/");
        pathB.Should().StartWith("tenant-globex/");
    }
}
