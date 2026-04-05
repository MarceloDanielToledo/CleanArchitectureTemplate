# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run

```bash
# Restore and build
dotnet restore
dotnet build CleanArchitectureTemplate.sln

# Run the API (auto-migrates and seeds data on startup)
dotnet run --project src/Presentation/WebAPI/WebAPI.csproj
```

## Testing

```bash
# Run all tests
dotnet test CleanArchitectureTemplate.sln

# Run a specific test project
dotnet test tests/Core/Application.UnitTests/Application.UnitTests.csproj
dotnet test tests/Infraestructure/Repository.UnitTests/Repository.UnitTests.csproj

# Run a single test class or method
dotnet test --filter "FullyQualifiedName~CreateOrderCommandHandlerTests"
```

## Architecture Overview

This is a .NET 8 Clean Architecture template with CQRS. The layers enforce a strict one-way dependency rule: `WebAPI → Application → Domain`, with `Repository` implementing contracts defined in `Application`, and `Shared` providing cross-cutting utilities.

### Layers

- **Domain** (`src/Core/Domain`): Pure entity classes with no dependencies. All entities extend `BaseEntity` (Id, CreatedOn, ModifiedOn auto-set by DbContext override). Anemic model — no business logic here.
- **Application** (`src/Core/Application`): All business logic. Defines CQRS commands/queries via MediatR, FluentValidation validators, repository contracts (`IRepositoryAsync<T>`), and response models. A `ValidationBehaviour<TRequest, TResponse>` pipeline behavior runs all validators before every handler.
- **Repository** (`src/Infraestructure/Repository`): EF Core implementation. Implements `IRepositoryAsync<T>` using Ardalis.Specification. DbContext overrides `SaveChangesAsync` to populate timestamps. Applies `IEntityTypeConfiguration<T>` from assembly scan.
- **WebAPI** (`src/Presentation/WebAPI`): Controllers, middleware, DI wiring. Exposes API routes as `api/v{version:apiVersion}/[controller]`. `ErrorHandlerMiddleware` catches all exceptions and maps them to the `Response<T>` wrapper.
- **Shared** (`src/Shared/Shared`): `IDateTimeService` and other cross-cutting utilities.

### CQRS Pattern

Every use case lives in `src/Core/Application/{Domain}/{UseCases}/`:
- `Commands/` — mutating operations (Create, Edit, Delete)
- `Queries/` — read operations (GetById, etc.)
- `Requests/` and `Responses/` — DTOs
- `Specifications/` — Ardalis.Specification classes for EF Core queries
- `Validators/` — FluentValidation rules
- `Mappings/` — mapping profiles

Handlers are `internal sealed class` to force DI usage. All return `Response<T>`.

### Dependency Injection Registration

Each layer registers itself via extension methods called in `Program.cs`:
- `AddApplicationServices()` — MediatR, AutoMapper (or manual mappings), validators, pipeline behaviors
- `AddRepositoryServices(config)` — DbContext (SQL Server), repositories (transient)
- `AddSharedServices()` — IDateTimeService

### Response Wrapper

All endpoints return `Response<T>` with `Succeeded`, `Message`, `Errors[]`, and `Data`. Use `Response<T>.Success(data)` and `Response<T>.NotSuccess(message, errors)` factory methods.

### Error Handling

`ErrorHandlerMiddleware` maps exceptions to HTTP codes:
- `ApplicationException` → 400
- `ValidationException` (FluentValidation) → 400 with error list
- `KeyNotFoundException` → 404
- Everything else → 500

Throw `KeyNotFoundException` for not-found cases and `ApplicationException` for business rule violations. Never catch in handlers — let the middleware handle it.

### Database & Migrations

- SQL Server in production (connection string via user secrets / appsettings)
- EF Core InMemory in `Repository.UnitTests`
- Migrations auto-applied on startup via `app.ApplyMigrations()`
- Seed data (10 products) loads after migrations via `SeedData.InitializeDataAsync()`

To add a new migration:
```bash
dotnet ef migrations add <MigrationName> --project src/Infraestructure/Repository --startup-project src/Presentation/WebAPI
```

### Testing Patterns

- **Application.UnitTests**: Mock `IRepositoryAsync<T>` with Moq, mock AutoMapper (or mapping contracts). Test handler success paths and exception scenarios.
- **Repository.UnitTests**: Use EF Core InMemory — no mocks. Test actual CRUD via `RepositoryAsync<T>`.
- **Presentation.IntegrationTests**: Scaffolded with `ApiWebApplication` test host; not fully implemented.
- Test data factories live in `tests/Shared/Shared` (e.g., `ProductHelper`, `OrderHelper`).
