# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run

```bash
# Restore and build
dotnet restore
dotnet build CleanArchitectureTemplate.sln

# Run the API (auto-migrates and seeds data on startup)
dotnet run --project src/Presentation/WebAPI/WebAPI.csproj

# Run with Docker (API + SQL Server 2022)
docker-compose up --build
```

## Testing

```bash
# Run all tests
dotnet test CleanArchitectureTemplate.sln

# Run a specific test project
dotnet test tests/Core/Application.UnitTests/Application.UnitTests.csproj
dotnet test tests/Infraestructure/Repository.UnitTests/Repository.UnitTests.csproj
dotnet test tests/Presentation/Presentation.IntegrationTests/Presentation.IntegrationTests.csproj

# Run a single test class or method
dotnet test --filter "FullyQualifiedName~CreateOrderCommandHandlerTests"
```

## Architecture Overview

This is a .NET 9 Clean Architecture template with CQRS. The layers enforce a strict one-way dependency rule: `WebAPI → Application → Domain`, with `Repository` implementing contracts defined in `Application`, and `Shared` providing cross-cutting utilities.

### Layers

- **Domain** (`src/Core/Domain`): Pure entity classes with no dependencies. All entities extend `BaseEntity` (Id, CreatedOn, ModifiedOn auto-set by DbContext override). Anemic model — no business logic here.
- **Application** (`src/Core/Application`): All business logic. Defines CQRS commands/queries using manual interfaces (`ICommand<T>`, `IQuery<T>`), FluentValidation validators, repository contracts (`IRepositoryAsync<T>`), and response models. A `ValidatingCommandHandler<TCommand, TResponse>` decorator wraps every command handler and runs all validators before execution.
- **Repository** (`src/Infraestructure/Repository`): EF Core implementation. Implements `IRepositoryAsync<T>` using Ardalis.Specification. DbContext overrides `SaveChangesAsync` to populate timestamps. Applies `IEntityTypeConfiguration<T>` from assembly scan.
- **WebAPI** (`src/Presentation/WebAPI`): Controllers, middleware, DI wiring. Exposes API routes as `api/v{version:apiVersion}/[controller]`. `ErrorHandlerMiddleware` catches all exceptions and maps them to the `Response<T>` wrapper.
- **Shared** (`src/Shared/Shared`): `IDateTimeService` and other cross-cutting utilities.

### CQRS Pattern

Manual CQRS — no external library. Interfaces live in `src/Core/Application/Abstractions/Messaging/`:
- `ICommand<TResponse>` — marker interface for commands
- `ICommandHandler<TCommand, TResponse>` — handler contract with `HandleAsync(command, ct)`
- `IQuery<TResponse>` — marker interface for queries
- `IQueryHandler<TQuery, TResponse>` — handler contract with `HandleAsync(query, ct)`

Every use case lives in `src/Core/Application/UseCases/{Domain}/`:
- `Commands/` — mutating operations (Create, Edit, Delete); command class + handler in the same file
- `Queries/` — read operations (GetById, etc.); query class + handler in the same file
- `Requests/` and `Responses/` — DTOs
- `Specifications/` — Ardalis.Specification classes for EF Core queries
- `Validators/` — FluentValidation rules
- `Mappings/` — static mapping extension methods (no external mapping library)

Handlers are `internal sealed class` to force DI usage. All return `Response<T>`.

**Validation**: `ValidatingCommandHandler<TCommand, TResponse>` (in `Behaviours/`) is a decorator registered automatically by DI. It resolves all `IValidator<TCommand>` from the container and throws `ValidationException` before delegating to the inner handler. Query handlers have no validation decorator.

### Dependency Injection Registration

Each layer registers itself via extension methods called in `Program.cs`:
- `AddApplicationServices()` — manual CQRS handler registration (assembly scan), FluentValidation validators. `ICommandHandler<,>` implementations are registered behind a `ValidatingCommandHandler<,>` decorator factory. `IQueryHandler<,>` implementations are registered directly.
- `AddRepositoryServices(config)` — DbContext (SQL Server by default, InMemory when `Database:UseInMemory=true`), repositories (transient)
- `AddSharedServices()` — IDateTimeService

Controllers inject specific handler interfaces by constructor — no mediator or service locator.

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
- EF Core InMemory when config key `Database:UseInMemory=true` (used by integration tests)
- Migrations auto-applied on startup via `app.ApplyMigrations()` — skipped when environment is `Testing`
- Seed data (10 products) loads after migrations — also skipped in `Testing` environment

To add a new migration:
```bash
dotnet ef migrations add <MigrationName> --project src/Infraestructure/Repository --startup-project src/Presentation/WebAPI
```

### API Documentation

Scalar UI replaces Swagger. Available at `/scalar/v1` in Development environment.
- `Microsoft.AspNetCore.OpenApi` — native .NET 9 OpenAPI document generation (`AddOpenApi()` / `MapOpenApi()`)
- `Scalar.AspNetCore` — interactive UI (`MapScalarApiReference()`)
- Response types documented via `[ProducesResponseType<T>]` on each action; common error codes (400, 500) on `BaseApiController`

### Testing Patterns

- **Application.UnitTests**: Mock `IRepositoryAsync<T>` with Moq. Instantiate handlers directly and call `HandleAsync(command, CancellationToken.None)`. Test success paths and exception scenarios.
- **Repository.UnitTests**: Use EF Core InMemory — no mocks. Test actual CRUD via `RepositoryAsync<T>`.
- **Presentation.IntegrationTests**: `WebApplicationFactory<Program>` spins up the full app in-process. DB is replaced with EF Core InMemory via `Database:UseInMemory=true` injected through `ConfigureHostConfiguration`. State resets between tests with `EnsureDeleted` + `EnsureCreated`. No external SQL Server required.
- Test data factories live in `tests/Shared/Shared` (compiled as `Tests.Shared.dll` to avoid name collision with `src/Shared/Shared`).
- Mappings are static extension methods (`ToEntity()`, `ToResponse()`) — no mocking needed.
- `Program.cs` exposes `public partial class Program {}` so `WebApplicationFactory<Program>` can reference the entry point.
