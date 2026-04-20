using AspNetCore.Multitenancy;
using AspNetCore.Multitenancy.Resolvers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace AspNetCore.Multitenancy.Tests.Unit;

public class TenantResolverTests
{
    private static TenantInfo MakeTenant(string id) =>
        new() { Id = id, Name = id, Plan = "free", IsActive = true };

    // ── HeaderTenantResolver ───────────────────────────────────────────────

    [Fact]
    public async Task Header_ValidTenantId_ReturnsTenant()
    {
        var store = new InMemoryTenantStore([MakeTenant("acme")]);
        var resolver = new HeaderTenantResolver(store, "X-Tenant-Id");

        var ctx = new DefaultHttpContext();
        ctx.Request.Headers["X-Tenant-Id"] = "acme";

        var result = await resolver.ResolveAsync(ctx);

        result.Should().NotBeNull();
        result!.Id.Should().Be("acme");
    }

    [Fact]
    public async Task Header_MissingHeader_ReturnsNull()
    {
        var store = new InMemoryTenantStore([MakeTenant("acme")]);
        var resolver = new HeaderTenantResolver(store);

        var result = await resolver.ResolveAsync(new DefaultHttpContext());

        result.Should().BeNull();
    }

    [Fact]
    public async Task Header_UnknownTenantId_ReturnsNull()
    {
        var store = new InMemoryTenantStore();
        var resolver = new HeaderTenantResolver(store);

        var ctx = new DefaultHttpContext();
        ctx.Request.Headers["X-Tenant-Id"] = "unknown";

        var result = await resolver.ResolveAsync(ctx);
        result.Should().BeNull();
    }

    [Fact]
    public async Task Header_InactiveTenant_ReturnsNull()
    {
        var tenant = MakeTenant("acme");
        tenant.IsActive = false;
        var store = new InMemoryTenantStore([tenant]);
        var resolver = new HeaderTenantResolver(store);

        var ctx = new DefaultHttpContext();
        ctx.Request.Headers["X-Tenant-Id"] = "acme";

        var result = await resolver.ResolveAsync(ctx);
        result.Should().BeNull();
    }

    // ── HostTenantResolver ─────────────────────────────────────────────────

    [Fact]
    public async Task Host_SubdomainMatch_ReturnsTenant()
    {
        var store = new InMemoryTenantStore([MakeTenant("acme")]);
        var resolver = new HostTenantResolver(store, "app.com");

        var ctx = new DefaultHttpContext();
        ctx.Request.Host = new HostString("acme.app.com");

        var result = await resolver.ResolveAsync(ctx);
        result.Should().NotBeNull();
        result!.Id.Should().Be("acme");
    }

    [Fact]
    public async Task Host_NoSubdomain_ReturnsNull()
    {
        var store = new InMemoryTenantStore();
        var resolver = new HostTenantResolver(store, "app.com");

        var ctx = new DefaultHttpContext();
        ctx.Request.Host = new HostString("app.com");

        var result = await resolver.ResolveAsync(ctx);
        result.Should().BeNull();
    }

    // ── PathTenantResolver ─────────────────────────────────────────────────

    [Fact]
    public async Task Path_ValidPrefix_ReturnsTenantAndRewritesPath()
    {
        var store = new InMemoryTenantStore([MakeTenant("acme")]);
        var resolver = new PathTenantResolver(store);

        var ctx = new DefaultHttpContext();
        ctx.Request.Path = "/acme/products";

        var result = await resolver.ResolveAsync(ctx);

        result.Should().NotBeNull();
        result!.Id.Should().Be("acme");
        ctx.Request.Path.Value.Should().Be("/products");
    }
}
