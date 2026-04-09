***English** | [Português](README.md)*

# RVM.MenuNaMao

Complete digital menu and restaurant management system with QR Code, Blazor Server admin panel and React electronic menu.

![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![React 19](https://img.shields.io/badge/React-19-61DAFB?logo=react)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?logo=postgresql)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-7.x-FF6600?logo=rabbitmq)
![License](https://img.shields.io/badge/License-Proprietary-red)

---

## About

**MenuNaMao** allows restaurants to create tables with QR Codes, manage their menu, receive orders directly from the customer's phone and track everything in real time through an admin panel. The system operates on two fronts:

- **Admin Panel (Blazor Server)** -- the restaurant owner manages tables, menu, orders and stock.
- **Electronic Menu (React SPA)** -- the customer scans the table's QR Code, browses the menu, builds a cart and places an order.

Domain events (order placed, status changed, low stock) are propagated via RabbitMQ for asynchronous processing.

---

## Technologies

| Layer | Technology | Version |
|---|---|---|
| Runtime | .NET | 10.0 |
| API / Web | ASP.NET Core + Blazor Server | 10.0 |
| ORM | Entity Framework Core | 10.0 |
| Database | PostgreSQL (Npgsql) | 10.0.1 |
| Messaging | RabbitMQ.Client | 7.2.1 |
| QR Code | QRCoder | 1.6.0 |
| Logging | Serilog.AspNetCore | 10.0.0 |
| Tests | xUnit + Moq + EF InMemory | 2.9.3 / 4.20.72 / 10.0 |
| Frontend | React + TypeScript + Vite | 19.0 / 5.7 / 6.0 |
| HTTP Client | Axios | 1.7.9 |
| SPA Routing | react-router-dom | 7.1.1 |

---

## Architecture

The project follows **Clean Architecture** with four layers and dependencies pointing inward:

```
+---------------------------------------------------------+
|                      API / Host                         |
|  ASP.NET Core  |  Blazor Server  |  Controllers  |  MW  |
+---------------------------------------------------------+
         |                    |
         v                    v
+---------------------------------------------------------+
|                    Infrastructure                       |
|  EF Core (Npgsql)  |  RabbitMQ  |  QRCoder  |  Repos   |
+---------------------------------------------------------+
         |
         v
+---------------------------------------------------------+
|                     Application                         |
|  Custom Mediator  |  Commands  |  Queries  |  Behaviors |
|  Notifications    |  DTOs      |  Services (interfaces) |
+---------------------------------------------------------+
         |
         v
+---------------------------------------------------------+
|                       Domain                            |
|  Entities  |  Enums  |  Repository Interfaces           |
+---------------------------------------------------------+

React SPA (menunomao-menu)  <----  API REST (CORS)  ---->  Blazor Admin
```

**Dependency rule:** Domain references nobody. Application references only Domain. Infrastructure references Application. API references Infrastructure.

---

## Custom Mediator

The project implements the Mediator pattern from scratch, without MediatR. The implementation lives in `Application/Mediator/`.

### Contracts

| Interface | Description |
|---|---|
| `IRequest<TResponse>` | Marks a command/query that returns `TResponse` |
| `IRequestHandler<TRequest, TResponse>` | Processes an `IRequest<TResponse>` |
| `INotification` | Marks a domain event (fire-and-forget) |
| `INotificationHandler<T>` | Processes a notification (there can be N handlers) |
| `IPipelineBehavior<TRequest, TResponse>` | Interceptor that wraps the handler execution |
| `IMediator` | Facade with `Send<T>()` and `Publish<T>()` |

### Pipeline Russian-Doll

When calling `mediator.Send(request)`, the Mediator builds the pipeline at runtime:

```
Request
  --> LoggingBehavior  (logs name + time)
      --> ValidationBehavior  (runs IValidator<T>, throws ValidationException)
          --> Handler real  (business logic)
          <-- Response
      <-- Response
  <-- Response
```

The behaviors are registered as open `IPipelineBehavior<,>` and resolved in reverse order to build the "Russian doll". The Mediator uses reflection to resolve generic handlers and behaviors from the DI container.

### Assembly Scanning

`DependencyInjection.AddMenuNaMaoApplication()` automatically scans the assembly and registers all `IRequestHandler<,>` and `INotificationHandler<>` found, as well as the pipeline behaviors in the correct order.

---

## Project Structure

```
RVM.MenuNaMao/
|-- RVM.MenuNaMao.slnx
|-- src/
|   |-- RVM.MenuNaMao.Domain/
|   |   |-- Entities/          Category, MenuItem, Order, OrderItem,
|   |   |                      Restaurant, StockItem, StockMovement, Table
|   |   |-- Enums/             OrderStatus, OrderItemStatus, TableStatus,
|   |   |                      StockMovementType, StockUnit
|   |   |-- Interfaces/        ICategoryRepository, IMenuItemRepository,
|   |                          IOrderRepository, IRestaurantRepository,
|   |                          IStockItemRepository, IStockMovementRepository,
|   |                          ITableRepository
|   |
|   |-- RVM.MenuNaMao.Application/
|   |   |-- Mediator/          IRequest, IRequestHandler, INotification,
|   |   |                      INotificationHandler, IPipelineBehavior,
|   |   |                      IMediator, Mediator, Unit
|   |   |-- Behaviors/         LoggingBehavior, ValidationBehavior
|   |   |-- Commands/          PlaceOrderCommand, CreateTableCommand,
|   |   |                      CreateCategoryCommand, CreateMenuItemCommand,
|   |   |                      CreateRestaurantCommand, UpdateOrderStatusCommand,
|   |   |                      AddStockMovementCommand
|   |   |-- Queries/           GetMenuQuery, GetOrderQuery, GetDashboardQuery,
|   |   |                      GetRestaurantOrdersQuery, GetStockItemsQuery,
|   |   |                      GetTableOrdersQuery, GetTablesQuery, ResolveTableQuery
|   |   |-- Notifications/     OrderPlacedNotification/Handler,
|   |   |                      OrderStatusChangedNotification/Handler,
|   |   |                      StockLowNotification/Handler
|   |   |-- DTOs/              CategoryDto, DashboardDto, MenuDto, MenuItemDto,
|   |   |                      OrderDto, OrderItemDto, RestaurantDto,
|   |   |                      StockItemDto, StockMovementDto, TableDto
|   |   |-- Services/          IQrCodeService, IRabbitMqPublisher
|   |   |-- DependencyInjection.cs
|   |
|   |-- RVM.MenuNaMao.Infrastructure/
|   |   |-- Data/              MenuNaMaoDbContext, Configurations/ (8 configs)
|   |   |-- Repositories/      CategoryRepository, MenuItemRepository,
|   |   |                      OrderRepository, RestaurantRepository,
|   |   |                      StockItemRepository, StockMovementRepository,
|   |   |                      TableRepository
|   |   |-- RabbitMQ/          RabbitMqPublisher, RabbitMqSettings,
|   |   |                      Consumers/ (OrderPlaced, OrderStatusChanged,
|   |   |                                  StockLowAlert)
|   |   |-- Services/          QrCodeService
|   |   |-- DependencyInjection.cs
|   |
|   |-- RVM.MenuNaMao.API/
|   |   |-- Controllers/       MenuController, OrdersController, TablesController,
|   |   |                      AdminCategoriesController, AdminDashboardController,
|   |   |                      AdminMenuItemsController, AdminOrdersController,
|   |   |                      AdminRestaurantsController, AdminStockController,
|   |   |                      AdminTablesController
|   |   |-- Components/Pages/  Dashboard, MenuPage, OrdersPage, TablesPage, StockPage
|   |   |-- Middleware/        CorrelationIdMiddleware, GlobalExceptionMiddleware
|   |   |-- Program.cs
|   |
|   |-- menunomao-menu/        (React 19 SPA)
|       |-- src/
|           |-- pages/         MenuPage, CartPage, CheckoutPage, OrderStatusPage
|           |-- context/       CartContext, OrderContext
|           |-- api/           client.ts (Axios)
|           |-- App.tsx, main.tsx
|
|-- test/
    |-- RVM.MenuNaMao.Tests/
        |-- Mediator/          MediatorTests
        |-- Handlers/          AddStockMovement, CreateTable, GetMenu,
        |                      PlaceOrder, UpdateOrderStatus
        |-- Repositories/      OrderRepositoryTests
        |-- Helpers/           TestDbContext
```

---

## How to Run

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL 16+](https://www.postgresql.org/download/)
- [RabbitMQ 3.13+](https://www.rabbitmq.com/download.html) (optional -- the system works without it, only logs a warning)
- [Node.js 20+](https://nodejs.org/) (for the React frontend)

### 1. Clone the repository

```bash
git clone https://github.com/rvenegas/RVM.MenuNaMao.git
cd RVM.MenuNaMao
```

### 2. Configure the database

Create the PostgreSQL database and adjust the connection string in `src/RVM.MenuNaMao.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Database=menunomao;Username=postgres;Password=postgres"
  }
}
```

### 3. Apply migrations (if any) or let EF create them

```bash
cd src/RVM.MenuNaMao.API
dotnet ef database update
```

### 4. Run the backend

```bash
dotnet run --project src/RVM.MenuNaMao.API
```

The server starts at `http://localhost:5000` (or the configured port). The Blazor panel is accessible at `/admin`.

### 5. Run the React frontend

```bash
cd src/menunomao-menu
npm install
npm run dev
```

Vite starts at `http://localhost:5173`. CORS is already configured for this origin.

### 6. RabbitMQ (optional)

If RabbitMQ is running at `localhost:5672` with user `guest/guest`, the consumers start automatically. Otherwise, the application starts normally and logs a warning.

---

## QR Code Flow

```
1. Admin creates a table in the panel (/admin/tables)
   |
   v
2. System generates a QrCodeToken (GUID v7) for the table
   |
   v
3. Admin downloads the QR Code (GET /api/admin/restaurants/{id}/tables/{id}/qrcode)
   The QR Code contains the URL: http://host/menu?table={token}
   |
   v
4. QR Code is printed and placed on the table
   |
   v
5. Customer scans it with their phone -> opens the React SPA
   |
   v
6. React calls GET /api/tables/resolve?token={token} to get the TableId
   |
   v
7. React loads the menu via GET /api/menu/{slug}
   |
   v
8. Customer builds the cart and confirms the order -> POST /api/orders
   |
   v
9. Backend creates the order and publishes OrderPlacedNotification to RabbitMQ
   |
   v
10. Admin sees the order in the panel (/admin/orders) and updates the status
```

---

## API Endpoints

### Client Endpoints (Public)

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/menu/{slug}` | Returns the full restaurant menu |
| `GET` | `/api/tables/resolve?token={token}` | Resolves QR Code token to table data |
| `POST` | `/api/orders` | Creates a new order |
| `GET` | `/api/orders/{id}` | Checks an order's status |
| `GET` | `/health` | Health check |

### Admin Endpoints

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/admin/restaurants` | Lists all restaurants |
| `POST` | `/api/admin/restaurants` | Creates a restaurant |
| `GET` | `/api/admin/restaurants/{id}/dashboard` | Dashboard with daily metrics |
| `GET` | `/api/admin/restaurants/{id}/tables` | Lists restaurant tables |
| `POST` | `/api/admin/restaurants/{id}/tables` | Creates a table |
| `GET` | `/api/admin/restaurants/{id}/tables/{tableId}/qrcode` | Downloads the table's QR Code (PNG) |
| `GET` | `/api/admin/restaurants/{id}/categories` | Lists categories |
| `POST` | `/api/admin/restaurants/{id}/categories` | Creates a category |
| `PUT` | `/api/admin/restaurants/{id}/categories/{catId}` | Updates a category |
| `DELETE` | `/api/admin/restaurants/{id}/categories/{catId}` | Deletes a category |
| `GET` | `/api/admin/restaurants/{id}/menu-items` | Lists menu items |
| `POST` | `/api/admin/restaurants/{id}/menu-items` | Creates a menu item |
| `PUT` | `/api/admin/restaurants/{id}/menu-items/{itemId}` | Updates an item |
| `DELETE` | `/api/admin/restaurants/{id}/menu-items/{itemId}` | Deletes an item |
| `GET` | `/api/admin/orders?restaurantId={id}&status={s}` | Lists orders (optional status filter) |
| `PUT` | `/api/admin/orders/{id}/status` | Updates order status |
| `GET` | `/api/admin/restaurants/{id}/stock` | Lists stock items |
| `POST` | `/api/admin/restaurants/{id}/stock` | Creates a stock item |
| `GET` | `/api/admin/stock/{stockItemId}/movements` | Lists stock movements |
| `POST` | `/api/admin/stock/{stockItemId}/movements` | Records a movement (entry/exit/adjustment) |

---

## Blazor Admin

Server-side rendered admin panel with Blazor Server interactivity.

| Route | Page | Functionality |
|---|---|---|
| `/admin` | Dashboard | Pending orders, preparing, daily total, revenue, stock alerts |
| `/admin/orders` | Orders | Order list with status filter, buttons to advance status (Prepare/Ready/Delivered) |
| `/admin/menu` | Menu | Create categories and items, view full menu |
| `/admin/tables` | Tables | Create tables, view status and token, download QR Code |
| `/admin/stock` | Stock | Create stock items, record entry/exit, visual alerts for low stock |

---

## React Menu

SPA accessed by the customer when scanning the table's QR Code. Built with React 19, TypeScript and Vite.

| Route | Page | Functionality |
|---|---|---|
| `/menu/:slug?table={token}` | MenuPage | Displays menu by categories, allows adding items to cart |
| `/cart` | CartPage | Views cart items, changes quantities |
| `/checkout` | CheckoutPage | Confirms order and sends it to the API |
| `/order/:orderId` | OrderStatusPage | Tracks order status in real time |

**React Contexts:**
- `CartContext` -- manages cart state (items, tableToken, restaurantSlug)
- `OrderContext` -- manages order state after checkout

---

## RabbitMQ

The system uses RabbitMQ 7.x (async API) for asynchronous communication via queues. The publisher is a singleton and the consumers are `BackgroundService`.

| Queue | Publisher | Consumer | Event |
|---|---|---|---|
| `orders.placed` | `OrderPlacedHandler` | `OrderPlacedConsumer` | New order placed |
| `orders.status-changed` | `OrderStatusChangedHandler` | `OrderStatusChangedConsumer` | Order status changed |
| `stock.low-alert` | `StockLowHandler` | `StockLowAlertConsumer` | Stock below minimum |

**Characteristics:**
- Queues declared as `durable: true` (persist through broker restarts)
- Manual ACK (`autoAck: false`) for delivery guarantee
- Resilient publisher: if RabbitMQ is offline, logs a warning and continues
- Resilient consumers: if they cannot connect, they log a warning and do not crash the application

---

## Tests

The project has **11 tests** organized in three categories:

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity normal
```

| Category | File | Tests |
|---|---|---|
| Mediator | `MediatorTests.cs` | Send resolves handler, Publish fan-out, Pipeline behavior |
| Handlers | `PlaceOrderHandlerTests.cs` | Creates order with items |
| Handlers | `CreateTableHandlerTests.cs` | Creates table with QR token |
| Handlers | `GetMenuHandlerTests.cs` | Returns full menu |
| Handlers | `UpdateOrderStatusHandlerTests.cs` | Updates status and publishes notification |
| Handlers | `AddStockMovementHandlerTests.cs` | Stock movement + low stock alert |
| Repositories | `OrderRepositoryTests.cs` | Order persistence and querying (EF InMemory) |

**Test stack:** xUnit 2.9.3 + Moq 4.20.72 + EF Core InMemory 10.0

---

## Features

- **Multi-restaurant** -- support for multiple restaurants with unique slug
- **Digital menu** -- ordered categories, items with price, description, image and preparation time
- **QR Code per table** -- automatic QR Code PNG generation with direct link to the menu
- **Real-time orders** -- customer places order from phone, admin tracks and updates status
- **Order status flow** -- Pending -> Preparing -> Ready -> Delivered / Cancelled
- **Stock control** -- items with unit of measurement, minimum quantity and automatic alerts
- **Stock movements** -- entry, exit and adjustment with full history
- **Operational dashboard** -- pending orders, preparing, daily total, revenue, stock alerts
- **Custom Mediator** -- custom implementation of the Mediator pattern with pipeline behaviors
- **Pipeline behaviors** -- automatic logging with execution time and pre-handler validation
- **Assembly scanning** -- automatic handler registration via reflection
- **Asynchronous messaging** -- RabbitMQ with singleton publisher and 3 BackgroundService consumers
- **Custom middleware** -- CorrelationId per request and global exception handling
- **Serilog** -- structured console logging
- **Health checks** -- `/health` endpoint for monitoring
- **Configured CORS** -- secure communication between React SPA and API
- **Automated tests** -- coverage of Mediator, handlers and repositories

---

## Middleware

| Middleware | Function |
|---|---|
| `CorrelationIdMiddleware` | Generates or propagates `X-Correlation-Id` on each request/response |
| `GlobalExceptionMiddleware` | Catches `ValidationException` (400), `KeyNotFoundException` (404) and generic errors (500) with standardized JSON response |

---

Developed by **RVM Tech**
