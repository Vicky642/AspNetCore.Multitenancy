# AspNetCore.Multitenancy

<div align="center">

[![CI](https://github.com/Vicky642/AspNetCore.Multitenancy/actions/workflows/ci.yml/badge.svg)](https://github.com/Vicky642/AspNetCore.Multitenancy/actions/workflows/ci.yml)
[![NuGet Package](https://img.shields.io/nuget/v/AspNetCore.Multitenancy.Core.svg)](https://www.nuget.org/packages/AspNetCore.Multitenancy.Core)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Sponsor Vicky642](https://img.shields.io/badge/Sponsor-Vicky642-brightgreen?logo=github)](https://github.com/sponsors/Vicky642)

**The Definitive Open Source Multi-Tenancy Toolkit for .NET**

</div>

## Overview

AspNetCore.Multitenancy is a complete, production-ready multi-tenancy foundation for ASP.NET Core 8+ applications. It covers everything from tenant resolution to per-tenant databases, background jobs, isolated file storage, and plan-based feature flags in one coherent package.

Each concern is a separate NuGet package so you only install what you need.

## Packages

| Package | Description |
| ------- | ----------- |
| `AspNetCore.Multitenancy.Core` | Base abstractions `ITenantContext`, `ITenantResolver`, `TenantInfo` |
| `AspNetCore.Multitenancy.Resolvers` | Resolvers: Header, Host (subdomain), JWT claim, Path |
| `AspNetCore.Multitenancy.EFCore` | Db-per-tenant, Schema-per-tenant, Row-level isolation strategies |
| `AspNetCore.Multitenancy.Hangfire` | Tenant-aware Hangfire background jobs (`TenantJobFilter`) |
| `AspNetCore.Multitenancy.Storage` | Tenant-isolated file storage (Local, S3, Azure Blob) |
| `AspNetCore.Multitenancy.Features` | Plan-based feature gating (`[RequireFeature]`) |

## Quick Start

### 1. Configure Services

In your `Program.cs`:

```csharp
builder.Services
    .AddMultitenancy(options =>
    {
        options.TenantStore = TenantStore.EntityFramework;
        options.IsolationStrategy = IsolationStrategy.DbPerTenant;
    })
    .WithHeaderResolver("X-Tenant-Id")
    .WithHostResolver()
    .WithEFCoreIsolation<AppDbContext>()
    .WithHangfireJobs()
    .WithTenantStorage(StorageProvider.S3);
```

### 2. Configure Middleware

```csharp
app.UseMultitenancy(); // resolves the tenant

app.UseRouting();
app.UseAuthorization();
```

### 3. Inject Context

```csharp
[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ITenantContext _tenantContext;
    
    public ProductsController(ITenantContext tenantContext)
    {
        _tenantContext = tenantContext;
    }
    
    [HttpGet]
    public IActionResult Get()
    {
        var tenant = _tenantContext.CurrentTenant;
        return Ok(new { Tenant = tenant.Name });
    }
}
```

## Sample Application

Check out the [`samples/SaaSDemo`](samples/SaaSDemo/) directory for a complete working SaaS demonstration showcasing all features.

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for details on how to contribute to the project.

## Support & Sponsorship

If this library saves your SaaS startup weeks of engineering time, please consider sponsoring the project!
Click the "Sponsor" button at the top of the repo.

### Tiers

- **Backer** ($5/mo)
- **Supporter** ($20/mo) - Priority issue response
- **Pro** ($100/mo) - Logo in README, 24h support
- **Enterprise** ($500/mo) - Dedicated Slack channel, roadmap input

## Supported By

Your logo could be here!
