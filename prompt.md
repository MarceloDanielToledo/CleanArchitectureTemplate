# Agent Prompt — Add "Get All Products" Endpoint

## Task

Implement a new endpoint `GET /api/v1/products` that returns the full list of products.

The codebase follows Clean Architecture with manual CQRS. Every new read operation requires:
1. A **Specification** (query filter)
2. A **Query + Handler** (use case)
3. A **Controller action** (presentation)
4. A **Unit test** (validation)

---

## What to implement

### 1. Specification
**File:** `src/Core/Application/UseCases/Products/Specifications/GetAllProductsSpecification.cs`

Return all products ordered by Id ascending.

### 2. Query + Handler
**File:** `src/Core/Application/UseCases/Products/Queries/GetAllProductsQuery.cs`

- Query: `GetAllProductsQuery` implements `IQuery<Response<IEnumerable<ProductResponse>>>`
- Handler: `GetAllProductsQueryHandler` — inject `IRepositoryAsync<Product>`, use the specification, map each entity with `ToResponse()`, return `Response<IEnumerable<ProductResponse>>.Success(data)`
- Throw `KeyNotFoundException` if the list is empty

### 3. Controller action
**File:** `src/Presentation/WebAPI/Controllers/v1/ProductsController.cs`

- Inject `IQueryHandler<GetAllProductsQuery, Response<IEnumerable<ProductResponse>>>` via constructor
- Add `GET` action at the base route (no extra segment)
- Return `Ok(result)`
- Decorate with `[ProducesResponseType<Response<IEnumerable<ProductResponse>>>(StatusCodes.Status200OK)]`

### 4. Unit test
**File:** `tests/Core/Application.UnitTests/QueryHandlers/GetAllProductsQueryHandlerTests.cs`

Write two tests:
- `Handle_Should_ReturnAllProducts_WhenProductsExist` — mock returns a list of 2 products, assert `Succeeded == true` and correct count
- `Handle_Should_ThrowNotFoundException_WhenNoProductsExist` — mock returns empty list, assert `KeyNotFoundException` is thrown

---

## Key conventions to follow

- Handler class must be `internal sealed class`
- No manual DI registration needed — assembly scan picks it up automatically
- Use `ProductMappings.ToResponse()` for entity → DTO mapping (already exists)
- Use `IRepositoryAsync<Product>.ListAsync(specification, cancellationToken)` to fetch the list
- Response wrapper: `Response<T>.Success(data)` — no custom message needed
- Do NOT add validation (only commands have validators, not queries)
