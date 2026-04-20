using AspNetCore.Multitenancy.FeatureFlags;
using FluentAssertions;
using Moq;
using Xunit;

namespace AspNetCore.Multitenancy.Tests.Unit;

public class QueryFilterTests
{
    private static ITenantContext MakeTenantContext(string plan)
    {
        var mock = new Mock<ITenantContext>();
        mock.Setup(c => c.HasTenant).Returns(true);
        mock.Setup(c => c.CurrentTenant).Returns(new TenantInfo { Id = "acme", Plan = plan });
        return mock.Object;
    }

    private static PlanFeatureOptions MakeOptions() =>
        new PlanFeatureOptions()
            .AddPlan("free",       ["BasicDashboard"])
            .AddPlan("pro",        ["BasicDashboard", "AdvancedReporting", "ApiAccess"])
            .AddPlan("enterprise", ["BasicDashboard", "AdvancedReporting", "ApiAccess", "WhiteLabel"]);

    [Theory]
    [InlineData("free",       "BasicDashboard",     true)]
    [InlineData("free",       "AdvancedReporting",  false)]
    [InlineData("pro",        "AdvancedReporting",  true)]
    [InlineData("pro",        "WhiteLabel",         false)]
    [InlineData("enterprise", "WhiteLabel",         true)]
    [InlineData("enterprise", "AdvancedReporting",  true)]
    public void IsEnabled_ReturnsCorrectResult(string plan, string feature, bool expected)
    {
        var ctx = MakeTenantContext(plan);
        var provider = new TenantPlanFeatureProvider(ctx, MakeOptions());

        var result = provider.IsEnabled(feature);

        result.Should().Be(expected);
    }

    [Fact]
    public void IsEnabled_NoTenant_ReturnsFalse()
    {
        var mock = new Mock<ITenantContext>();
        mock.Setup(c => c.HasTenant).Returns(false);
        var provider = new TenantPlanFeatureProvider(mock.Object, MakeOptions());

        var result = provider.IsEnabled("AdvancedReporting");

        result.Should().BeFalse();
    }

    [Fact]
    public void GetEnabledFeatures_Pro_ReturnsCorrectSet()
    {
        var ctx = MakeTenantContext("pro");
        var provider = new TenantPlanFeatureProvider(ctx, MakeOptions());

        var features = provider.GetEnabledFeatures();

        features.Should().Contain("BasicDashboard")
                         .And.Contain("AdvancedReporting")
                         .And.Contain("ApiAccess")
                         .And.NotContain("WhiteLabel");
    }
}
