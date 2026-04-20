# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.1.0] - 2026-04-20

### Added

- **Core**: `ITenantContext`, `ITenantResolver`, `TenantInfo` model, `TenantMiddleware`, tenant store abstraction
- **Resolvers**: Header, Host (subdomain), Claims (JWT), and Path-based tenant resolution strategies
- **EF Core**: `MultitenantDbContext` with three isolation strategies:
  - Database-per-tenant
  - Schema-per-tenant
  - Row-level security with global query filters
- **Background Jobs**: Tenant-aware Hangfire integration with `TenantJobFilter` and `TenantRecurringJobManager`
- **File Storage**: `ITenantStorageProvider` with Local, AWS S3, and Azure Blob providers
- **Feature Flags**: Plan-based feature gating with `[RequireFeature]` attribute
- **Sample App**: Complete SaaS demo with tenant CRUD, products, and background jobs
- **Tests**: Unit, integration, and E2E test projects
- **CI/CD**: GitHub Actions workflows for build, release, and docs deployment
