# QuickOrder — Instrucciones para Claude

## Stack
- .NET 9 / ASP.NET Core
- Entity Framework Core 9 + PostgreSQL (Npgsql)
- MediatR (CQRS)
- FluentValidation
- SignalR
- AWS SQS
- Razor Pages (frontend dentro de QuickOrder.Api)

## Arquitectura — Clean Architecture

```
QuickOrder.Domain        → Entidades, Enums
QuickOrder.Application   → Interfaces, DTOs, Commands, Queries, Validators, Behaviors, Messages
QuickOrder.Infrastructure → DbContext, Repositories, Migrations, Hubs, Messaging
QuickOrder.Api           → Controllers, Middleware, Pages (Razor), Program.cs
QuickOrder.Worker        → BackgroundService (consume SQS, notifica via SignalR)
```

## Reglas obligatorias

### Siempre usar repositorios
- Los handlers NUNCA usan `AppDbContext` o `IAppDbContext` directamente.
- Siempre inyectar interfaces de repositorio (`IOrderRepository`, `IMenuProductRepository`, etc.) + `IUnitOfWork`.
- Toda query compleja va en el repositorio, no en el handler.

### Pipeline MediatR
1. `ValidationBehavior` — corre primero, lanza `ValidationException` si hay errores
2. `LoggingBehavior` — loguea nombre del request, tiempo, errores

### Respuestas API
- Todos los endpoints retornan `ApiResponse<T>`.
- Éxito: `ApiResponse<T>.Ok(data, message?)`
- Error: `ApiResponse<object>.Fail(message, errors?)`
- El `ExceptionMiddleware` intercepta excepciones y las convierte:
  - `ValidationException` → 400 con lista de errores
  - `KeyNotFoundException` → 404
  - `ArgumentException` → 400
  - Cualquier otra → 500

### Validaciones
- Cada command tiene su validator con FluentValidation en `Features/.../Validators/`.

## Modelo de dominio

### Multi-menú
- `Menu` — ej. Salón, Vereda
- `Product` — catálogo maestro (sin precio, sin disponibilidad)
- `MenuProduct` — vincula Menu + Product + Category, tiene `Price` e `IsAvailable`
- `Category` — agrupa `MenuProduct`s, tiene `DisplayOrder`

### Modificadores (gustos/adicionales)
- `ModifierGroup` — grupo de opciones (ej. "Cocción", "Extras")
  - `IsRequired`, `MinSelections`, `MaxSelections`
  - Se asigna a un `Product` (vía `ProductId`) O a una `Category` (vía `CategoryId`)
- `Modifier` — opción dentro del grupo (ej. "A punto", "Bien cocida")
- `MenuModifier` — override de precio/disponibilidad por menú
- `OrderItemModifier` — modificadores elegidos al momento del pedido

### Órdenes
- `Order` tiene `TableId` + `MenuId`
- `OrderItem` tiene `MenuProductId` (no `ProductId`), `UnitPrice` (copiado al momento del pedido)
- `OrderItemModifier` tiene `ExtraPrice` (copiado al momento del pedido)
- `OrderStatusHistory` registra cada cambio de estado

## Convenciones

- Commands/Queries en `Application/Features/{Area}/Commands|Queries/`
- DTOs en `Application/DTOs/`
- Interfaces de repositorio en `Application/Interfaces/`
- Implementaciones en `Infrastructure/Repositories/`
- Registrar todos los repositorios en `Infrastructure/DependencyInjection.cs`
- Configuraciones EF en `Infrastructure/Persistence/Configurations/OrderConfiguration.cs` (un archivo, múltiples clases)
- Al agregar entidades nuevas: agregar `DbSet` en `AppDbContext` y en `IAppDbContext`

## AWS SQS
- Queue: configurar via variable de entorno `Sqs__QueueUrl` o `appsettings.local.json`
- Region: `us-east-1`
- El Worker conecta como cliente SignalR al hub `/hubs/orders` de la API

## Frontend (Razor Pages)
- Vive en `QuickOrder.Api/Pages/`
- Shared layout en `Pages/Shared/_Layout.cshtml`
- Páginas actuales: `Index` (tablero cocina), `History` (historial de pedidos)
