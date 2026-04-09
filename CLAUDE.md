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

### Contrato de IUnitOfWork
```csharp
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

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

### Manejo de errores en handlers
- **Usar Result pattern** para errores esperados (entidad no encontrada, validación de negocio, argumento inválido).
- Los handlers retornan `Result<T>` o `Result` (para void) en lugar de lanzar excepciones.
- Usar `Error.NotFound(msg)`, `Error.Validation(msg)`, `Error.Business(msg)` para construir errores.
- El `ExceptionMiddleware` **se mantiene** solo para errores inesperados (infraestructura, excepciones no controladas → 500) y `ValidationException` de FluentValidation (→ 400).
- Los controllers heredan de `ApiController` y usan `ToResponse(result)` para convertir el Result a IActionResult.
- Para creaciones: verificar `result.IsFailure` antes de llamar `CreatedAtAction`, luego usar `result.Value!`.

### Validaciones
- Cada command tiene su validator con FluentValidation en `Features/.../Validators/`.

### DTOs
- Usar `record` para DTOs de queries (inmutables, solo lectura).
- Usar `record` también para commands salvo que necesiten lógica de construcción compleja.
- Los DTOs **nunca** exponen entidades del dominio directamente.

### Archivos por feature
- Cada command/query tiene su propio archivo: `CreateOrderCommand.cs`, `CreateOrderCommandHandler.cs`.
- El handler va en un archivo separado del command, en la misma carpeta.

## Ejemplo de Command + Handler

```csharp
// Application/Features/Orders/Commands/CreateOrderCommand.cs
public record CreateOrderCommand(int TableId, int MenuId) : IRequest<Result<OrderDto>>;

// Application/Features/Orders/Validators/CreateOrderCommandValidator.cs
public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.TableId).GreaterThan(0);
        RuleFor(x => x.MenuId).GreaterThan(0);
    }
}

// Application/Features/Orders/Commands/CreateOrderCommandHandler.cs
public class CreateOrderCommandHandler(
    IOrderRepository orderRepository,
    IMenuRepository menuRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateOrderCommand, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var menu = await menuRepository.GetByIdAsync(request.MenuId, cancellationToken)
            ?? throw new KeyNotFoundException($"Menu {request.MenuId} not found.");

        var order = new Order(request.TableId, menu.Id);
        await orderRepository.AddAsync(order, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}
```

## Ejemplo de Query + Handler

```csharp
// Application/Features/Orders/Queries/GetOrderByIdQuery.cs
public record GetOrderByIdQuery(int OrderId) : IRequest<OrderDto>;

// Application/Features/Orders/Queries/GetOrderByIdQueryHandler.cs
public class GetOrderByIdQueryHandler(
    IOrderRepository orderRepository) : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdWithDetailsAsync(request.OrderId, cancellationToken)
            ?? throw new KeyNotFoundException($"Order {request.OrderId} not found.");

        return new OrderDto(order.Id, order.TableId, order.Status);
    }
}
```

## Ejemplo de Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class OrdersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return Ok(ApiResponse<int>.Ok(id, "Orden creada exitosamente."));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetOrderByIdQuery(id), ct);
        return Ok(ApiResponse<OrderDto>.Ok(result));
    }
}
```

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
- Configuraciones EF en `Infrastructure/Persistence/Configurations/` (un archivo por entidad, ej. `OrderConfiguration.cs`)
- Al agregar entidades nuevas: agregar `DbSet` en `AppDbContext` y en `IAppDbContext`

## AWS SQS
- Queue: configurar via variable de entorno `Sqs__QueueUrl` o `appsettings.local.json`
- Region: `us-east-1`
- El Worker conecta como cliente SignalR al hub `/hubs/orders` de la API

### Flujo del Worker
1. Consume mensajes de SQS con `long polling`
2. Deserializa el mensaje a un tipo de evento (ej. `OrderStatusChangedMessage`)
3. Conecta al hub SignalR de la API (`/hubs/orders`) como cliente y emite el evento al grupo correspondiente (ej. por `tableId` o broadcast a cocina)
4. El Worker **no tiene su propio MediatR** ni accede a la base de datos directamente — solo retransmite eventos al hub

## Frontend (Razor Pages)
- Vive en `QuickOrder.Api/Pages/`
- Shared layout en `Pages/Shared/_Layout.cshtml`
- Páginas actuales: `Index` (tablero cocina), `History` (historial de pedidos)
- Usan Tag Helpers de ASP.NET Core
- Los PageModels llaman a `IMediator` para queries, siguiendo el mismo pipeline que los controllers

## Anti-patrones — nunca hacer esto
- ❌ Usar `AppDbContext` directamente en un handler
- ❌ Lógica de negocio en Controllers o PageModels
- ❌ DTOs que exponen entidades del dominio
- ❌ Lanzar `KeyNotFoundException` o `ArgumentException` desde handlers (usar `Result.Fail(Error.NotFound(...))` en su lugar)
- ❌ Handlers que hacen más de una cosa (un handler = un caso de uso)
- ❌ `.Result` / `.Wait()` en código async
- ❌ `new` de dependencias concretas dentro de clases (viola DI)
