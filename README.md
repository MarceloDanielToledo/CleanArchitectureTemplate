[![es](https://img.shields.io/badge/lang-es-red)](https://github.com/MarceloDanielToledo/CleanArchitectureTemplate/blob/main/LICENSE)

# Clean Architecture Template ⚡

This repository provides a starting point for developing APIs in .NET following the principles of **Clean Architecture**. Designed to be modular, scalable, and easy to maintain, this template facilitates the development of robust and well-structured applications.

---

## Context

The management of orders was designed with a clear and simple structure:

- **Orders**: Represent a transaction that groups one or more items.
- **Items**: These are specific products within an order, indicating quantity and unit price.
- **Products**: Contain basic information about available items, such as name, description, and price. *The application performs a seed during initialization to load 10 products.

```csharp
public class SeedData
{
    public static async Task InitializeDataAsync(IServiceProvider serviceProvider)
    {
        await SeedProducts.Seed(serviceProvider);
    }
}
```

---

## 🛠️ Features

- CQRS pattern implementation using MediaTr.
- Request validations using FluentValidation.
- Data seeding for product insertion during project initialization.
- Exception Middleware to centralize all application errors.
- _AutoMigration_ on application startup.
- IEntityTypeConfiguration: We adhere to the single responsibility principle by defining each entity configuration in separate classes.
- Generic HTTP response wrapper for all responses.
- Route design following **RESTful** principles.

---

## 🏗️ Project Structure

```plaintext
📂 src
 ├── 📁 Core
 │    ├─ 📘 Application
 │    ├─ 📘 Domain
 ├── 📁 Infraestructure
 │    ├─ 📘 Repository
 ├── 📁 Presentation
 │    ├─ 📘 WebAPI
 └── 📁 Shared
      ├─ 📘 Shared
📂 tests
 ├── 📁 Core
 │    ├─ ✅ Application.UnitTests
 ├── 📁 Infraestructure
 │    ├─ ✅ Repository.UnitTests
 ├── 📁 Presentation
 │    ├─ 🔍 Presentation.IntegrationTests [Coming soon 🚀]
 └── 📁 Shared
```

---

### Domain

Defines the business model with completely "anemic" entities, meaning they do not contain validations or behaviors. This allows validations and business logic to be managed in the upper layer (Application).

### Application

Contains business logic specific to each use case, defining validations, mappings, DTOs, API interfaces, exceptions, commands, and queries.

### Repository

Defines any type of persistence, which could be a database, files, etc. This layer implements the interface responsible for input and output operations.

### WebAPI

The outermost layer that communicates the application with the external world. It defines endpoints, the _ErrorHandlerMiddleware_, and adds services from all layers via extension methods.

### Shared

Contains shared code between layers that doesn’t belong to any specific one, such as services, utility classes, etc.

---

## ✅ Tests

**Application**: Unit tests are created for each command, such as:

```C#
[Fact]
public async Task Handle_Should_ReturnFailure_WhenMappingFails()
{
    // Arrange
    var createOrderRequest = new CreateOrderRequest
    {
        Comment = "Test order",
        OrderItems = new List<CreateOrderItemRequest>
        {
            new CreateOrderItemRequest { ProductId = 1, Quantity = 2, UnitPrice = 100 }
        }
    };
    var command = new CreateOrderCommand(createOrderRequest);

    // Mock the mapper to throw an exception
    _mapperMock.Setup(x => x.Map<Order>(createOrderRequest))
        .Throws(new Exception("Mapping error"));

    var handler = new CreateOrderCommandHandler(
        _orderRepositoryMock.Object, 
        _productRepositoryMock.Object, 
        _mapperMock.Object
    );

    // Act & Assert
    var exception = await Assert.ThrowsAsync<Exception>(
        () => handler.Handle(command, CancellationToken.None)
    );
    Assert.Equal("Mapping error", exception.Message);
}

```

**Repository**: Tests use Microsoft.EntityFrameworkCore.InMemory:

``` C#
public RepositoryAsyncTests()
{
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: "TestDatabase")
        .Options;

    _dbContext = new ApplicationDbContext(options, new Mock<IDateTimeService>().Object);
    _orderRepository = new RepositoryAsync<OrderItem>(_dbContext);
    _orderItemRepository = new RepositoryAsync<OrderItem>(_dbContext);
    _productRepository = new RepositoryAsync<Product>(_dbContext);
}

```

---

## Libraries Used:

- Entity Framework Core
- Ardalis.Specification
- AutoMapper
- FluentValidation
- MediatR
- Ardalis
- Serilog.AspNetCore
- IHttpClientFactory
- Polly
- Microsoft.EntityFramework.Core
- Microsoft.EntityFrameworkCore.InMemory
- Microsoft.EntityFrameworkCore.SqlServer
- Swashbuckle.AspNetCore
- Serilog
- Moq
- xUnit


---

Consider giving a ⭐ star, forking the repository, and staying tuned for updates!


