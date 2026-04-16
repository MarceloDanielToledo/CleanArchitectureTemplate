[![en](https://img.shields.io/badge/lang-en-red)](https://github.com/MarceloDanielToledo/CleanArchitectureTemplate/blob/main/README.md)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple)](https://dotnet.microsoft.com)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

# Clean Architecture Template 

<img src="assets/banner.png" alt="Clean Architecture Template Banner" width="100%"/>

Este repositorio proporciona un punto de partida para desarrollar APIs en .NET siguiendo los principios de la **Arquitectura Limpia** (Clean Architecture). Diseñado para ser modular, escalable y fácil de mantener, este template facilita el desarrollo de aplicaciones robustas y bien estructuradas.

## ¿Por qué este template?

Un starter de Clean Architecture práctico y liviano, diseñado para adaptarse a cualquier equipo.
Cada patrón es explícito e implementado manualmente — sin magia oculta, sin convenciones impuestas,
fácil de adaptar a las necesidades de tu equipo desde el primer día.

---

## Contexto del dominio

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

---

## 🛠️ Características

- **CQRS sin magia** — comandos y queries conectados manualmente, sin dependencia en librerías externas.
- **Validación como decorador** — `ValidatingCommandHandler<,>` ejecuta todas las reglas de FluentValidation antes de cada handler.
- **Sin librerías de mapeo externas** — métodos de extensión estáticos (`ToEntity()`, `ToResponse()`) que podés leer y depurar directamente.
- **Auto-migración y seeding** — la app migra la base de datos y carga 10 productos de muestra en cada inicio.
- **Manejo centralizado de errores** — `ErrorHandlerMiddleware` mapea todas las excepciones a respuestas HTTP consistentes.
- **`IEntityTypeConfiguration`** — la configuración de EF Core de cada entidad vive en su propia clase (Responsabilidad Única).
- **Wrapper de respuesta genérico** — cada endpoint retorna `Response<T>` con `Succeeded`, `Message`, `Errors[]` y `Data`.
- **Rutas RESTful** — las rutas siguen convenciones REST (`api/v{version}/[controller]`).

---

## Requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server o SQL Server Express — **o Docker** (ver abajo)

## Inicio rápido

### Local

```bash
git clone https://github.com/MarceloDanielToledo/CleanArchitectureTemplate.git
cd CleanArchitectureTemplate
dotnet restore
dotnet run --project src/Presentation/WebAPI/WebAPI.csproj
```

La API auto-migra la base de datos y carga 10 productos de muestra en el primer arranque.
La UI de Scalar está disponible en `http://localhost:{port}/scalar/v1`.

### Docker

```bash
docker-compose up --build
```

Levanta la API y una instancia de SQL Server 2022. No requiere instalación local de SQL Server.

| Servicio | URL |
|---------|-----|
| API | `http://localhost:8080` |
| Scalar UI | `http://localhost:8080/scalar/v1` |

**Detener:**
```bash
docker-compose down
```

**Detener y eliminar datos:**
```bash
docker-compose down -v
```

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
 │    ├─ ✅ Presentation.IntegrationTests
 └── 📁 Shared
```

### Domain

Aquí definimos el modelo de negocio con entidades totalmente "anémicas", lo que significa que no contienen validaciones ni comportamientos. Esto permite que las validaciones y lógica de negocio sean gestionadas en la capa superior (Application).

### Application

Contiene la lógica de negocio específica para cada caso de uso, definiendo validaciones, mapeos, DTOs, interfaces de la API, excepciones, comandos y queries.

### Repository

Capa que define cualquier tipo de persistencia que puede ser en una base de datos, en ficheros, etc. En esta capa se debe implementar la interfaz encargada de las operaciones de entrada y salidas.

### WebAPI

Esta capa es la más periférica y es la que comunica la aplicación con el mundo exterior, aquí definiremos nuestros endpoints, el *ErrorHandlerMiddleware* y añadiremos los servicios de todas las capas mediante métodos de extensión.

### Shared

En esta capa tendremos todo el código compartido entre capas no perteneciente a ninguna, como servicios, clases auxiliares, etc.

---

## ✅ Tests

**Application**: Los tests unitarios instancian los handlers directamente (sin contenedor DI ni librerías externas). Los repositorios se mockean con Moq. Cada command handler tiene al menos un test de éxito y uno de fallo.

Ver [`tests/Core/Application.UnitTests`](tests/Core/Application.UnitTests) para ejemplos.

**Repository**: Los tests utilizan `Microsoft.EntityFrameworkCore.InMemory` — sin mocks, comportamiento real de EF Core contra una base de datos en memoria.

Ver [`tests/Infraestructure/Repository.UnitTests`](tests/Infraestructure/Repository.UnitTests) para ejemplos.

**Presentation** (Integración): Usa `WebApplicationFactory<Program>` para levantar el stack real de la aplicación en proceso. La base de datos se reemplaza por un provider en memoria — no requiere SQL Server externo. El estado se resetea entre tests con `EnsureDeleted` + `EnsureCreated`.

Ver [`tests/Presentation/Presentation.IntegrationTests`](tests/Presentation/Presentation.IntegrationTests) para ejemplos.

---

## Librerías utilizadas

| Librería | Propósito |
|---------|---------|
| Entity Framework Core | ORM y migraciones de base de datos |
| Ardalis.Specification | Especificaciones para queries del repositorio |
| FluentValidation | Validación de entrada de comandos |
| Asp.Versioning.Mvc | Versionado de la API |
| Microsoft.AspNetCore.OpenApi | Generación nativa de documentos OpenAPI (.NET 9) |
| Scalar.AspNetCore | UI interactiva de documentación de la API |
| Serilog | Logging estructurado |
| Moq | Mocking para tests unitarios |
| xUnit | Framework de tests unitarios |
| Microsoft.EntityFrameworkCore.InMemory | Base de datos en memoria para testing |

---

## Contribuciones

¡Las contribuciones son bienvenidas! Haz un fork del repositorio, crea una rama y abre un pull request.

## Licencia

Este proyecto está licenciado bajo la licencia MIT. Consultá el archivo [`LICENSE`](LICENSE) para más detalles.
