# AGENTS.md

This file provides guidance to AI coding agents when working with code in this repository.

> **Parity with CLAUDE.md**: this file and `CLAUDE.md` must be kept in sync. Every change to `AGENTS.md` must be replicated in `CLAUDE.md` and vice versa.

> [!IMPORTANT]
> **LANGUAGE INSTRUCTION FOR AGENTS**: Although this file is written in English, the Agent MUST ALWAYS respond to the user in **SPANISH**.
> **IMPLEMENTATION PLAN**: The proposed implementation plan MUST ALWAYS be in **SPANISH**.

---

## Available Skills

Skills live in the `skills/` folder at the root of the repository. Invoke with `/skill-name` in Claude Code:

| Skill | Description |
|---|---|
| `/dotnet-backend-patterns` | C#/.NET backend patterns: DI, EF Core, async/await, caching, testing. **Auto-invoked** on backend code changes. |
| `/serilog` | Structured logging with Serilog for .NET 10: bootstrap, appsettings config, enrichers, sinks, request logging. |
| `/skill-creator` | Creates new agent skills following the Agent Skills spec. |
| `/skill-sync` | Syncs skill metadata to AGENTS.md auto-invoke sections. |

---

## Quick Commands

```bash
dotnet restore
dotnet build CleanArchitectureTemplate.sln
dotnet run --project src/Presentation/WebAPI/WebAPI.csproj
dotnet test CleanArchitectureTemplate.sln
dotnet test --filter "FullyQualifiedName~CreateOrderCommandHandlerTests"
dotnet ef migrations add <Name> --project src/Infraestructure/Repository --startup-project src/Presentation/WebAPI
docker-compose up --build
```

---

## Project Map

```
src/
  Core/
    Domain/          → Entities only. No dependencies.
    Application/     → All business logic. CQRS handlers, validators, specs, mappings.
  Infraestructure/
    Repository/      → EF Core DbContext, configurations, migrations, seeders.
  Presentation/
    WebAPI/          → Controllers, middleware, DI wiring, Program.cs.
  Shared/
    Shared/          → IDateTimeService / DateTimeService.
    Shared.Telemetry/→ OpenTelemetry setup.
tests/
  Core/Application.UnitTests/          → Unit tests (Moq). xUnit.
  Infraestructure/Repository.UnitTests/→ EF InMemory tests. xUnit.
  Presentation/Presentation.IntegrationTests/ → WebApplicationFactory + NUnit.
  Shared/Shared/                       → Test helpers: OrderHelper, ProductHelper, OrderItemHelper.
```

**Dependency rule (one-way, strict):** `WebAPI → Application → Domain`. Repository implements Application contracts. Shared has no deps.

---

## Domain Model

### Entities (`src/Core/Domain/`)

```
BaseEntity                       (src/Core/Domain/Contracts/BaseEntity.cs)
  virtual int Id
  DateTime CreatedOn             ← set by DbContext on Add
  DateTime? ModifiedOn           ← set by DbContext on Update

Order : BaseEntity               (Entities/Order.cs)
  string Comment
  List<OrderItem> OrderItems

OrderItem : BaseEntity           (Entities/OrderItem.cs)
  int OrderId / Order Order
  int ProductId / Product Product
  int Quantity
  decimal UnitPrice

Product : BaseEntity             (Entities/Product.cs)
  string Name
  string Description
  decimal Price
  int StockQuantity
  bool IsActive
```

### DB Constraints (Repository/Configurations/)

| Entity | Constraint |
|---|---|
| Order.Comment | MaxLength(300) |
| Product.Name | MaxLength(200), Required |
| Product.Description | MaxLength(300), Required |
| Product.Price | decimal(18,2), Required |
| OrderItem.UnitPrice | decimal(18,2), Required |
| Order → OrderItems | Cascade delete |
| OrderItem → Product | Restrict delete |

---

## API Surface

Base route: `api/v{version:apiVersion}/[controller]` — default version **1.0**.

| Method | Route | Handler |
|---|---|---|
| POST | /api/v1/order | CreateOrderCommandHandler |
| PUT | /api/v1/order/{id} | EditOrderCommandHandler |
| DELETE | /api/v1/order/{id} | DeleteOrderCommandHandler |
| GET | /api/v1/order/{id} | GetOrderByIdQueryHandler |
| POST | /api/v1/order/{orderId}/item | CreateOrderItemCommandHandler |
| PUT | /api/v1/order/{orderId}/item/{id} | EditOrderItemCommandHandler |
| DELETE | /api/v1/order/{orderId}/item/{id} | DeleteOrderItemCommandHandler |
| GET | /api/v1/order/{orderId}/item/{id} | GetOrderItemByIdQueryHandler |
| POST | /api/v1/products | CreateProductCommandHandler |
| PUT | /api/v1/products/{id} | EditProductCommandHandler |
| GET | /api/v1/products/{id} | GetProductByIdQueryHandler |

Scalar UI (Development only): `/scalar/v1`

---

## CQRS — Adding a New Use Case

All use cases live under `src/Core/Application/UseCases/{Domain}/`.

### Checklist — Command

```
UseCases/{Domain}/
  Commands/      CreateXxxCommand.cs   ← ICommand<Response<XxxResponse>> + internal sealed handler
  Requests/      CreateXxxRequest.cs
  Responses/     XxxResponse.cs        ← reuse if exists
  Validators/    CreateXxxRequestValidator.cs
  Mappings/      XxxMappings.cs        ← static ToEntity() / ToResponse()
  Specifications/GetXxxByIdSpecification.cs  ← if querying
```

**Handler skeleton:**
```csharp
public record CreateXxxCommand(CreateXxxRequest Request) : ICommand<Response<XxxResponse>>;

internal sealed class CreateXxxCommandHandler(IRepositoryAsync<Xxx> repository)
    : ICommandHandler<CreateXxxCommand, Response<XxxResponse>>
{
    public async Task<Response<XxxResponse>> HandleAsync(CreateXxxCommand command, CancellationToken ct)
    {
        var entity = command.Request.ToEntity();
        await repository.AddAsync(entity, ct);
        return Response<XxxResponse>.Success(entity.ToResponse(), ResponseMessages.AddedSuccesfullyMessage);
    }
}
```

### Checklist — Query

```
UseCases/{Domain}/
  Queries/       GetXxxByIdQuery.cs    ← IQuery<Response<XxxResponse>> + internal sealed handler
  Specifications/GetXxxByIdSpecification.cs
```

**No validation decorator on queries.** Throw `KeyNotFoundException` when not found.

### DI Registration (`src/Core/Application/Extensions/ServiceCollectionExtensions.cs`)

Commands are **auto-wrapped** with `ValidatingCommandHandler<,>`. Queries are registered directly. Both are discovered by assembly scan — **no manual registration needed** if you follow the naming convention.

---

## Key Files Reference

| File | Purpose |
|---|---|
| `src/Presentation/WebAPI/Program.cs` | App bootstrap, middleware, DI calls |
| `src/Core/Application/Extensions/ServiceCollectionExtensions.cs` | CQRS + FluentValidation DI |
| `src/Infraestructure/Repository/Extensions/ServiceCollectionExtensions.cs` | DbContext + repo DI |
| `src/Infraestructure/Repository/Contexts/ApplicationDbContext.cs` | EF Core DbContext |
| `src/Presentation/WebAPI/Middleware/ErrorHandlerMiddleware.cs` | Global exception handler |
| `src/Core/Application/Wrappers/Response.cs` | Response<T> wrapper |
| `src/Core/Application/Behaviours/ValidatingCommandHandler.cs` | Validation decorator |
| `src/Core/Application/Constants/ResponseMessages.cs` | Standard response strings |
| `src/Infraestructure/Repository/Seeders/SeedData.cs` | Startup seed (10 products) |
| `tests/Presentation/Presentation.IntegrationTests/ApiWebApplication.cs` | WebApplicationFactory |
| `tests/Presentation/Presentation.IntegrationTests/TestBase.cs` | NUnit base with reset |

---

## Validation Rules

Validation runs **before** command handlers via `ValidatingCommandHandler`. Validators registered automatically from Application assembly.

| Validator | Rules |
|---|---|
| CreateOrderRequestValidator | Comment: Length(1,300); each OrderItem: valid (ProductId/Quantity/UnitPrice > 0) |
| CreateProductRequestValidator | Name: NotEmpty, Length(1,200); Description: NotEmpty, Length(1,300); Price > 0; StockQuantity > 0 |
| EditProductRequestValidator | Id > 0 + same rules as Create |

---

## Response & Error Conventions

```csharp
// Success
Response<T>.Success(data)
Response<T>.Success(data, ResponseMessages.AddedSuccesfullyMessage)

// Failure
Response<T>.NotSuccess(message)
Response<T>.NotSuccess(message, errors)

// Exceptions → HTTP codes (via ErrorHandlerMiddleware)
throw new KeyNotFoundException(...)     // → 404
throw new ApplicationException(...)     // → 400
// ValidationException thrown by decorator  → 400 + Errors[]
// Unhandled                                → 500
```

**Never catch exceptions in handlers.** Let the middleware handle them.

---

## Testing

### Unit Tests (`tests/Core/Application.UnitTests/`)

```csharp
// Pattern: mock repo, instantiate handler directly
var repo = new Mock<IRepositoryAsync<Product>>();
repo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Product>>(), ct))
    .ReturnsAsync(ProductHelper.Create());
var handler = new GetProductByIdQueryHandler(repo.Object);
var result = await handler.HandleAsync(new GetProductByIdQuery(1), CancellationToken.None);
```

### Repository Tests (`tests/Infraestructure/Repository.UnitTests/`)

EF Core InMemory — no mocks. Test actual CRUD via `RepositoryAsync<T>`.

### Integration Tests (`tests/Presentation/Presentation.IntegrationTests/`)

```csharp
// Inherit TestBase (NUnit). DB resets before each test.
var client = CreateClient();
var response = await client.PostAsJsonAsync("/api/v1/products", request);
response.EnsureSuccessStatusCode();
```

**Test helpers** (`tests/Shared/Shared/`):
- `ProductHelper.Create()` → valid Product entity
- `OrderHelper.Create()` → Order with 5 OrderItems
- `OrderItemHelper.Create()` / `CreateList(n)`

---

## Configuration

| Key | Default | Effect |
|---|---|---|
| `ConnectionStrings:DefaultConnection` | SQLEXPRESS local | SQL Server connection |
| `Database:UseInMemory` | `true` (Development) | Swap to EF InMemory DB |

`Database:UseInMemory=true` → InMemory DB named `"IntegrationTestDb"`. Used in Development and all tests.

Migrations auto-applied on startup. Seed runs if Products table is empty. Both skipped when `ASPNETCORE_ENVIRONMENT=Testing`.

---

## Known Issues in Codebase

| Location | Issue |
|---|---|
| `ApplicationDbContext.cs` | `DbSet<OrderItem> Orders` — type should be `DbSet<Order> Orders` |
| `DeleteOrderCommandHandler` | Class named `DelteOrderCommandHandler` (missing 'e') |
| `ResponseMessages.cs` | `NotFoundsMessage` has a typo (extra 's') — prefer `NotFoundMessage` |
| `ApplicationException.cs` | Uses obsolete `Exception(SerializationInfo, StreamingContext)` — SYSLIB0051 warning |
