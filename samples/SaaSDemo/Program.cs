using AspNetCore.Multitenancy;
using AspNetCore.Multitenancy.EntityFrameworkCore;
using AspNetCore.Multitenancy.FeatureFlags;
using AspNetCore.Multitenancy.FileStorage;
using AspNetCore.Multitenancy.Resolvers;
using Microsoft.EntityFrameworkCore;
using SaaSDemo.Products;
using SaaSDemo.Tenants;

var builder = WebApplication.CreateBuilder(args);

// ── 1. Multi-tenancy (8 lines as per the spec) ────────────────────────────
builder.Services
    .AddMultitenancy(options =>
    {
        options.TenantStore = TenantStore.InMemory;
        options.IsolationStrategy = IsolationStrategy.RowLevel;
    })
    .WithHeaderResolver("X-Tenant-Id")   // curl -H "X-Tenant-Id: acme" ...
    .WithHostResolver()                   // acme.localhost → tenant "acme"
    .WithEFCoreIsolation<ProductsDbContext>((opts, cs) => opts.UseSqlServer(cs))
    .WithLocalStorage()                   // tenant-isolated local file storage
    .WithFeatureFlags(plans =>
    {
        plans.AddPlan("free",       ["BasicDashboard"]);
        plans.AddPlan("pro",        ["BasicDashboard", "AdvancedReporting", "ApiAccess"]);
        plans.AddPlan("enterprise", ["BasicDashboard", "AdvancedReporting", "ApiAccess", "WhiteLabel", "SSOIntegration"]);
    });

// ── 2. EF Core ─────────────────────────────────────────────────────────────
builder.Services.AddDbContext<ProductsDbContext>((sp, opts) =>
{
    // Row-level strategy: all tenants share one connection string
    opts.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

// ── 3. Tenant provisioning ─────────────────────────────────────────────────
builder.Services.AddScoped<TenantProvisioningService>();

// ── 4. Swagger ─────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "SaaS Demo API", Version = "v1" });
    c.AddSecurityDefinition("TenantId", new()
    {
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name = "X-Tenant-Id",
        Description = "Tenant identifier header",
    });
});

builder.Services.AddControllers();

// ── 5. Seed demo tenants ───────────────────────────────────────────────────
builder.Services.AddSingleton<ITenantStore>(new InMemoryTenantStore([
    new TenantInfo { Id = "acme",   Name = "Acme Corp",     Plan = "enterprise", IsActive = true },
    new TenantInfo { Id = "globex", Name = "Globex Inc",    Plan = "pro",        IsActive = true },
    new TenantInfo { Id = "initech",Name = "Initech LLC",   Plan = "free",       IsActive = true },
]));

var app = builder.Build();

// ── 6. Pipeline ────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMultitenancy();       // resolve tenant from header / host
app.UseTenantFeatures();     // enforce [RequireFeature] → 404 for disabled features

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
