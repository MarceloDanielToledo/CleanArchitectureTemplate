[![en](https://img.shields.io/badge/lang-en-red)](https://github.com/MarceloDanielToledo/CleanArchitectureTemplate/blob/main/README.md)

# Clean Architecture Template ⚡

Este repositorio proporciona un punto de partida para desarrollar APIs en .NET siguiendo los principios de la **Arquitectura Limpia** (Clean Architecture). Diseñado para ser modular, escalable y fácil de mantener, este template facilita el desarrollo de aplicaciones robustas y bien estructuradas.


---

## Contexto

Se diseñó la gestión de órdenes con una estructura clara y sencilla:

- **Órdenes**: Representan una transacción que agrupa uno o más items.
- **Items**: Son los productos específicos dentro de una orden, indicando cantidad y precio unitario.
- **Productos**: Contienen la información básica de los artículos disponibles, como nombre, descripción y precio. *La aplicación realizará un seed al inicializarse para cargar 10 productos*

```C#
public class SeedData
{
    public static async Task InitializeDataAsync(IServiceProvider serviceProvider)
    {
        await SeedProducts.Seed(serviceProvider);
    }
}
```

![[Pasted image 20250118180915.png]]

---

## 🛠️ Características

- Utilización del patrón CQRS mediante MediaTr
- Validaciones de request utilizando FluentValidation.
- Seed de datos para inserción de productos al inicializar el proyecto.
- Exception Middleware para centralizar todos los errores de la aplicación.
- *AutoMigration* al inicializar la aplicación.
- IEntityTypeConfiguration: Respetaremos el principio único de responsabilidad dejando la configuración de cada entidad en distintas clases para cada una.
- Uso de de response genérico para todas las respuestas HTTP.
- Diseño de rutas utilizando el principio  **RESTful**


---

## 🏗️ Estructura del Proyecto

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

### Domain

Aquí definimos el modelo de negocio con entidades totalmente "anémicas", lo que significa que no contienen validaciones ni comportamientos. Esto permite que las validaciones y lógica de negocio sean gestionadas en la capa superior (Application).
### Application

Contiene la lógica de negocio específica para cada caso de uso, definiendo validaciones, mapeos, DTOs, interfaces de la  API, excepciones, comandos y queries.

### Repository

Capa que define cualquier tipo de persistencia que puede ser en una base de datos, en ficheros, etc. En esta capa se debe implementar la interfaz encargada de las operaciones de entrada y salidas.
### WebAPI

Esta capa es la más periférica y es la que comunica la aplicación con el mundo exterior, aquí definiremos nuestros endpoints, el *ErrorHandlerMiddleware* y añadiremos los servicios de todas las capas mediante métodos de extensión.

### Shared

En esta capa tendremos todo el código compartido entre capas no perteneciente a ninguna, como servicios, clases auxiliares, etc.

---

## ✅ Tests

**Application**: vamos a realizar los casos de tests unitarios para cada command, por ejemplo:

```csharp
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

**Repository**: Realizaremos los tests utilizando Microsof.EntityFrameworkCore.InMemory

```csharp
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

## Librerías utilizadas:

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
- Microsoft.EntityFramework.Core.InMemory
- Microsoft.EntityFramework.Core.SqlServer
- Swashbuckle.AspNetCore
- Serilog
- Moq
- xUnit

---

Consider giving a star ⭐, forking the repository, and staying tuned for updates!
