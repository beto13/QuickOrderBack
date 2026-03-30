# QuickOrder — Backend API

Sistema de gestión de pedidos para restaurantes, construido con **.NET 9** y **Clean Architecture**. Permite tomar pedidos desde mesas, gestionar múltiples menús, personalizar productos con modificadores y hacer seguimiento en tiempo real del estado de cada orden.

---

## Características

- Gestión de pedidos con ciclo de vida completo: `Pendiente → En preparación → Listo → Entregado`
- Soporte multi-menú (distintas áreas o turnos con precios independientes)
- Modificadores por producto (extras, opciones de cocción, etc.)
- Dashboard de cocina en tiempo real vía **SignalR**
- Notificaciones asíncronas vía **AWS SQS** + Worker Service
- Historial de pedidos paginado
- Validaciones con **FluentValidation**
- Documentación interactiva con **Scalar / OpenAPI**

---

## Stack tecnológico

| Capa | Tecnología |
|---|---|
| Lenguaje | C# / .NET 9 |
| Web Framework | ASP.NET Core 9 |
| Arquitectura | Clean Architecture + CQRS |
| ORM | Entity Framework Core 9 |
| Base de datos | PostgreSQL 16 |
| Mensajería | AWS SQS |
| Tiempo real | SignalR |
| Validación | FluentValidation |
| Mediador | MediatR |
| Logging | Serilog |
| Contenedores | Docker + docker-compose |
| Frontend | Razor Pages |

---

## Estructura del proyecto

```
QuickOrder/
├── QuickOrder.Domain/          # Entidades, Enums, Excepciones
├── QuickOrder.Application/     # CQRS, DTOs, Interfaces, Validators, Behaviors
├── QuickOrder.Infrastructure/  # DbContext, Repositorios, SQS, SignalR Hub
├── QuickOrder.Api/             # Controllers, Middleware, Razor Pages, Program.cs
├── QuickOrder.Worker/          # Background service (consume SQS, notifica via SignalR)
└── QuickOrder.Tests/           # Unit tests (Handlers, Validators)
```

---

## Modelo de dominio

| Entidad | Descripción |
|---|---|
| `Table` | Mesa del restaurante |
| `Menu` | Contexto de menú (ej. Salón, Vereda, Mediodía) |
| `Product` | Catálogo maestro de productos |
| `MenuProduct` | Producto disponible en un menú con precio propio |
| `Category` | Agrupación de productos dentro de un menú |
| `Order` | Pedido de una mesa |
| `OrderItem` | Línea de un pedido (producto + cantidad + precio) |
| `ModifierGroup` | Grupo de opciones de personalización |
| `Modifier` | Opción individual (ej. "Término medio") |
| `OrderItemModifier` | Modificador seleccionado en un item |
| `OrderStatusHistory` | Historial de cambios de estado |

---

## API Endpoints

### Pedidos — `/api/orders`
| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/api/orders/active` | Pedidos activos |
| `POST` | `/api/orders` | Crear pedido |
| `PATCH` | `/api/orders/{id}/status` | Actualizar estado |
| `GET` | `/api/orders/history` | Historial paginado |

### Menús — `/api/menus`
| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/api/menus` | Listar menús |
| `GET` | `/api/menus/{id}` | Obtener menú |
| `POST` | `/api/menus` | Crear menú |
| `PUT` | `/api/menus/{id}` | Actualizar menú |
| `DELETE` | `/api/menus/{id}` | Desactivar menú |

> Endpoints similares para `/api/products`, `/api/tables`, `/api/categories`, `/api/modifier-groups`, `/api/modifiers`.

### Formato de respuesta

```json
{
  "success": true,
  "data": {},
  "message": null,
  "errors": null
}
```

---

## Arquitectura

```
Request → Controller → MediatR
                         ├── ValidationBehavior (FluentValidation)
                         ├── LoggingBehavior (Serilog)
                         └── Handler → Repository → DbContext (PostgreSQL)
                                    ↓
                              SqsMessagePublisher → AWS SQS
                                                       ↓
                                                   OrderWorker
                                                       ↓
                                               SignalR Hub → Clientes
```

---

## Levantar con Docker

```bash
docker-compose up --build
```

Servicios:

| Servicio | Puerto | Descripción |
|---|---|---|
| `api` | `8080` | API REST + Razor Pages |
| `worker` | — | Consume SQS, emite SignalR |
| `postgres` | `5432` | PostgreSQL 16 |

La base de datos se crea y migra automáticamente al iniciar.

---

## Variables de entorno

| Variable | Descripción |
|---|---|
| `ConnectionStrings__DefaultConnection` | Cadena de conexión PostgreSQL |
| `Sqs__QueueUrl` | URL de la cola SQS |
| `Sqs__Region` | Región AWS (ej. `us-east-1`) |
| `SignalR__HubUrl` | URL del hub SignalR (solo Worker) |

---

## Desarrollo local

### Requisitos

- .NET 9 SDK
- PostgreSQL 16
- (Opcional) Docker

### Configuración

1. Clonar el repositorio:
   ```bash
   git clone https://github.com/beto13/QuickOrderBack.git
   cd QuickOrderBack
   ```

2. Configurar `appsettings.Development.json` con la cadena de conexión local.

3. Aplicar migraciones:
   ```bash
   dotnet ef database update --project QuickOrder.Infrastructure --startup-project QuickOrder.Api
   ```

4. Ejecutar la API:
   ```bash
   dotnet run --project QuickOrder.Api
   ```

5. Abrir la documentación: `http://localhost:{puerto}/scalar`

---

## Tests

```bash
dotnet test
```

Cobertura:
- Handlers de comandos y queries de Orders y Products
- Validators con casos válidos e inválidos
