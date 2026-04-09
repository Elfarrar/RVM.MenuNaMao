# RVM.MenuNaMao

Sistema completo de cardapio digital e gestao de restaurante com QR Code, painel administrativo Blazor Server e menu eletronico React.

![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![React 19](https://img.shields.io/badge/React-19-61DAFB?logo=react)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?logo=postgresql)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-7.x-FF6600?logo=rabbitmq)
![License](https://img.shields.io/badge/License-Proprietary-red)

---

## Sobre

O **MenuNaMao** permite que restaurantes criem mesas com QR Code, gerenciem cardapio, recebam pedidos diretamente pelo celular do cliente e acompanhem tudo em tempo real por um painel administrativo. O sistema opera em duas frentes:

- **Painel Admin (Blazor Server)** -- o dono do restaurante gerencia mesas, cardapio, pedidos e estoque.
- **Menu Eletronico (React SPA)** -- o cliente escaneia o QR Code da mesa, navega pelo cardapio, monta o carrinho e faz o pedido.

Eventos de dominio (pedido criado, status alterado, estoque baixo) sao propagados via RabbitMQ para processamento assincrono.

---

## Tecnologias

| Camada | Tecnologia | Versao |
|---|---|---|
| Runtime | .NET | 10.0 |
| API / Web | ASP.NET Core + Blazor Server | 10.0 |
| ORM | Entity Framework Core | 10.0 |
| Banco de dados | PostgreSQL (Npgsql) | 10.0.1 |
| Mensageria | RabbitMQ.Client | 7.2.1 |
| QR Code | QRCoder | 1.6.0 |
| Logging | Serilog.AspNetCore | 10.0.0 |
| Testes | xUnit + Moq + EF InMemory | 2.9.3 / 4.20.72 / 10.0 |
| Frontend | React + TypeScript + Vite | 19.0 / 5.7 / 6.0 |
| HTTP Client | Axios | 1.7.9 |
| Roteamento SPA | react-router-dom | 7.1.1 |

---

## Arquitetura

O projeto segue **Clean Architecture** com quatro camadas e dependencias apontando para dentro:

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

**Regra de dependencia:** Domain nao referencia ninguem. Application referencia apenas Domain. Infrastructure referencia Application. API referencia Infrastructure.

---

## Custom Mediator

O projeto implementa o pattern Mediator do zero, sem MediatR. A implementacao vive em `Application/Mediator/`.

### Contratos

| Interface | Descricao |
|---|---|
| `IRequest<TResponse>` | Marca um command/query que retorna `TResponse` |
| `IRequestHandler<TRequest, TResponse>` | Processa um `IRequest<TResponse>` |
| `INotification` | Marca um evento de dominio (fire-and-forget) |
| `INotificationHandler<T>` | Processa uma notificacao (pode haver N handlers) |
| `IPipelineBehavior<TRequest, TResponse>` | Interceptor que envolve a execucao do handler |
| `IMediator` | Fachada com `Send<T>()` e `Publish<T>()` |

### Pipeline Russian-Doll

Ao chamar `mediator.Send(request)`, o Mediator monta a pipeline em tempo de execucao:

```
Request
  --> LoggingBehavior  (loga nome + tempo)
      --> ValidationBehavior  (executa IValidator<T>, lanca ValidationException)
          --> Handler real  (logica de negocio)
          <-- Response
      <-- Response
  <-- Response
```

Os behaviors sao registrados como `IPipelineBehavior<,>` aberto e resolvidos na ordem inversa para montar o "Russian doll". O Mediator usa reflection para resolver handlers e behaviors genericos do container DI.

### Assembly Scanning

`DependencyInjection.AddMenuNaMaoApplication()` escaneia o assembly automaticamente e registra todos os `IRequestHandler<,>` e `INotificationHandler<>` encontrados, alem dos pipeline behaviors na ordem correta.

---

## Estrutura do Projeto

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

## Como Executar

### Pre-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL 16+](https://www.postgresql.org/download/)
- [RabbitMQ 3.13+](https://www.rabbitmq.com/download.html) (opcional -- o sistema funciona sem, apenas loga warning)
- [Node.js 20+](https://nodejs.org/) (para o frontend React)

### 1. Clonar o repositorio

```bash
git clone https://github.com/rvenegas/RVM.MenuNaMao.git
cd RVM.MenuNaMao
```

### 2. Configurar o banco de dados

Crie o banco PostgreSQL e ajuste a connection string em `src/RVM.MenuNaMao.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Database=menunomao;Username=postgres;Password=postgres"
  }
}
```

### 3. Aplicar migrations (se existirem) ou deixar o EF criar

```bash
cd src/RVM.MenuNaMao.API
dotnet ef database update
```

### 4. Rodar o backend

```bash
dotnet run --project src/RVM.MenuNaMao.API
```

O servidor sobe em `http://localhost:5000` (ou a porta configurada). O painel Blazor fica acessivel em `/admin`.

### 5. Rodar o frontend React

```bash
cd src/menunomao-menu
npm install
npm run dev
```

O Vite sobe em `http://localhost:5173`. O CORS ja esta configurado para essa origem.

### 6. RabbitMQ (opcional)

Se o RabbitMQ estiver rodando em `localhost:5672` com usuario `guest/guest`, os consumers iniciam automaticamente. Caso contrario, a aplicacao sobe normalmente e loga um warning.

---

## Fluxo QR Code

```
1. Admin cria mesa no painel (/admin/tables)
   |
   v
2. Sistema gera QrCodeToken (GUID v7) para a mesa
   |
   v
3. Admin faz download do QR Code (GET /api/admin/restaurants/{id}/tables/{id}/qrcode)
   O QR Code contem a URL: http://host/menu?table={token}
   |
   v
4. QR Code eh impresso e colocado na mesa
   |
   v
5. Cliente escaneia com o celular -> abre o React SPA
   |
   v
6. React chama GET /api/tables/resolve?token={token} para obter TableId
   |
   v
7. React carrega o cardapio via GET /api/menu/{slug}
   |
   v
8. Cliente monta o carrinho e confirma o pedido -> POST /api/orders
   |
   v
9. Backend cria o pedido, publica OrderPlacedNotification no RabbitMQ
   |
   v
10. Admin ve o pedido no painel (/admin/orders) e atualiza o status
```

---

## Endpoints da API

### Endpoints do Cliente (Public)

| Metodo | Rota | Descricao |
|---|---|---|
| `GET` | `/api/menu/{slug}` | Retorna cardapio completo do restaurante |
| `GET` | `/api/tables/resolve?token={token}` | Resolve QR Code token para dados da mesa |
| `POST` | `/api/orders` | Cria um novo pedido |
| `GET` | `/api/orders/{id}` | Consulta status de um pedido |
| `GET` | `/health` | Health check |

### Endpoints Admin

| Metodo | Rota | Descricao |
|---|---|---|
| `GET` | `/api/admin/restaurants` | Lista todos os restaurantes |
| `POST` | `/api/admin/restaurants` | Cria um restaurante |
| `GET` | `/api/admin/restaurants/{id}/dashboard` | Dashboard com metricas do dia |
| `GET` | `/api/admin/restaurants/{id}/tables` | Lista mesas do restaurante |
| `POST` | `/api/admin/restaurants/{id}/tables` | Cria uma mesa |
| `GET` | `/api/admin/restaurants/{id}/tables/{tableId}/qrcode` | Download do QR Code da mesa (PNG) |
| `GET` | `/api/admin/restaurants/{id}/categories` | Lista categorias |
| `POST` | `/api/admin/restaurants/{id}/categories` | Cria uma categoria |
| `PUT` | `/api/admin/restaurants/{id}/categories/{catId}` | Atualiza uma categoria |
| `DELETE` | `/api/admin/restaurants/{id}/categories/{catId}` | Remove uma categoria |
| `GET` | `/api/admin/restaurants/{id}/menu-items` | Lista itens do cardapio |
| `POST` | `/api/admin/restaurants/{id}/menu-items` | Cria um item do cardapio |
| `PUT` | `/api/admin/restaurants/{id}/menu-items/{itemId}` | Atualiza um item |
| `DELETE` | `/api/admin/restaurants/{id}/menu-items/{itemId}` | Remove um item |
| `GET` | `/api/admin/orders?restaurantId={id}&status={s}` | Lista pedidos (filtro opcional por status) |
| `PUT` | `/api/admin/orders/{id}/status` | Atualiza status do pedido |
| `GET` | `/api/admin/restaurants/{id}/stock` | Lista itens de estoque |
| `POST` | `/api/admin/restaurants/{id}/stock` | Cria item de estoque |
| `GET` | `/api/admin/stock/{stockItemId}/movements` | Lista movimentacoes de estoque |
| `POST` | `/api/admin/stock/{stockItemId}/movements` | Registra movimentacao (entrada/saida/ajuste) |

---

## Blazor Admin

Painel administrativo renderizado server-side com interatividade Blazor Server.

| Rota | Pagina | Funcionalidade |
|---|---|---|
| `/admin` | Dashboard | Pedidos pendentes, preparando, total do dia, receita, alertas de estoque |
| `/admin/orders` | Pedidos | Lista de pedidos com filtro por status, botoes para avancar status (Preparar/Pronto/Entregue) |
| `/admin/menu` | Cardapio | Criar categorias e itens, visualizar cardapio completo |
| `/admin/tables` | Mesas | Criar mesas, ver status e token, download do QR Code |
| `/admin/stock` | Estoque | Criar itens de estoque, registrar entrada/saida, alertas visuais para estoque baixo |

---

## React Menu

SPA que o cliente acessa ao escanear o QR Code da mesa. Construido com React 19, TypeScript e Vite.

| Rota | Pagina | Funcionalidade |
|---|---|---|
| `/menu/:slug?table={token}` | MenuPage | Exibe cardapio por categorias, permite adicionar itens ao carrinho |
| `/cart` | CartPage | Visualiza itens no carrinho, altera quantidades |
| `/checkout` | CheckoutPage | Confirma pedido e envia para a API |
| `/order/:orderId` | OrderStatusPage | Acompanha status do pedido em tempo real |

**Contextos React:**
- `CartContext` -- gerencia estado do carrinho (itens, tableToken, restaurantSlug)
- `OrderContext` -- gerencia estado do pedido apos checkout

---

## RabbitMQ

O sistema utiliza RabbitMQ 7.x (async API) para comunicacao assincrona via filas. O publisher eh singleton e os consumers sao `BackgroundService`.

| Fila | Publisher | Consumer | Evento |
|---|---|---|---|
| `orders.placed` | `OrderPlacedHandler` | `OrderPlacedConsumer` | Novo pedido criado |
| `orders.status-changed` | `OrderStatusChangedHandler` | `OrderStatusChangedConsumer` | Status do pedido alterado |
| `stock.low-alert` | `StockLowHandler` | `StockLowAlertConsumer` | Estoque abaixo do minimo |

**Caracteristicas:**
- Filas declaradas como `durable: true` (persistem restart do broker)
- ACK manual (`autoAck: false`) para garantia de entrega
- Publisher resiliente: se o RabbitMQ estiver offline, loga warning e continua
- Consumers resilientes: se nao conseguirem conectar, logam warning e nao derrubam a aplicacao

---

## Testes

O projeto possui **11 testes** organizados em tres categorias:

```bash
# Rodar todos os testes
dotnet test

# Rodar com output detalhado
dotnet test --verbosity normal
```

| Categoria | Arquivo | Testes |
|---|---|---|
| Mediator | `MediatorTests.cs` | Send resolve handler, Publish fan-out, Pipeline behavior |
| Handlers | `PlaceOrderHandlerTests.cs` | Cria pedido com itens |
| Handlers | `CreateTableHandlerTests.cs` | Cria mesa com QR token |
| Handlers | `GetMenuHandlerTests.cs` | Retorna cardapio completo |
| Handlers | `UpdateOrderStatusHandlerTests.cs` | Atualiza status e publica notificacao |
| Handlers | `AddStockMovementHandlerTests.cs` | Movimentacao de estoque + alerta low stock |
| Repositories | `OrderRepositoryTests.cs` | Persistencia e consulta de pedidos (EF InMemory) |

**Stack de testes:** xUnit 2.9.3 + Moq 4.20.72 + EF Core InMemory 10.0

---

## Funcionalidades

- **Multi-restaurante** -- suporte a multiplos restaurantes com slug unico
- **Cardapio digital** -- categorias ordenadas, itens com preco, descricao, imagem e tempo de preparo
- **QR Code por mesa** -- geracao automatica de QR Code PNG com link direto para o menu
- **Pedidos em tempo real** -- cliente faz pedido pelo celular, admin acompanha e atualiza status
- **Fluxo de status do pedido** -- Pending -> Preparing -> Ready -> Delivered / Cancelled
- **Controle de estoque** -- itens com unidade de medida, quantidade minima e alertas automaticos
- **Movimentacoes de estoque** -- entrada, saida e ajuste com historico completo
- **Dashboard operacional** -- pedidos pendentes, preparando, total do dia, receita, alertas de estoque
- **Custom Mediator** -- implementacao propria do pattern Mediator com pipeline behaviors
- **Pipeline behaviors** -- logging automatico com tempo de execucao e validacao pre-handler
- **Assembly scanning** -- registro automatico de handlers via reflection
- **Mensageria assincrona** -- RabbitMQ com publisher singleton e 3 consumers BackgroundService
- **Middleware customizado** -- CorrelationId por request e tratamento global de excecoes
- **Serilog** -- logging estruturado em console
- **Health checks** -- endpoint `/health` para monitoramento
- **CORS configurado** -- comunicacao segura entre React SPA e API
- **Testes automatizados** -- cobertura de Mediator, handlers e repositorios

---

## Middleware

| Middleware | Funcao |
|---|---|
| `CorrelationIdMiddleware` | Gera ou propaga `X-Correlation-Id` em cada request/response |
| `GlobalExceptionMiddleware` | Captura `ValidationException` (400), `KeyNotFoundException` (404) e erros genericos (500) com resposta JSON padronizada |

---

Desenvolvido por **RVM Tech**
