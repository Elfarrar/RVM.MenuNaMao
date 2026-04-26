# RVM.MenuNaMao

## Visao Geral
Plataforma de cardapio digital via QR Code com duas interfaces: painel admin Blazor Server para restaurantes gerenciarem categorias, itens e pedidos; e app React (menunomao-menu) para clientes escanearem o QR Code, montarem carrinho e finalizarem pedido. Comunicacao assíncrona via RabbitMQ para processamento de pedidos.

Projeto portfolio demonstrando arquitetura CQRS com MediatR, integração RabbitMQ, SPA embarcada no ASP.NET Core e fluxo completo de pedido digital.

## Stack
- .NET 10, ASP.NET Core, Blazor Server (admin)
- React 18 + Vite + TypeScript (menunomao-menu — cardapio do cliente)
- MediatR (CQRS: Commands, Queries, Notifications)
- RabbitMQ (processamento assíncrono de pedidos)
- Entity Framework Core + PostgreSQL
- Serilog + Seq, RVM.Common.Security
- xUnit 61 testes, Playwright E2E

## Estrutura do Projeto
```
src/
  RVM.MenuNaMao.API/           # Host: Blazor admin + REST controllers
    Components/                # Blazor pages (cardapios, itens, pedidos, QR)
    Controllers/               # REST endpoints consumidos pelo React
    Middleware/                # CorrelationId, GlobalException
  RVM.MenuNaMao.Application/   # CQRS (MediatR)
    Commands/                  # CreateItem, CreateOrder, UpdateOrderStatus...
    Queries/                   # GetMenu, GetOrders, GetCategories...
    Behaviors/                 # Pipeline behaviors (validacao, logging)
    Notifications/             # Eventos de dominio
    Services/                  # Contratos de aplicacao
  RVM.MenuNaMao.Domain/        # Entidades (Category, MenuItem, Order, QrCode)
  RVM.MenuNaMao.Infrastructure/
    Data/                      # MenuNaMaoDbContext
    Repositories/              # Implementacoes
  menunomao-menu/              # React SPA (cardapio cliente, carrinho, checkout)
test/
  RVM.MenuNaMao.Tests/         # xUnit (61 testes)
  playwright/                  # Testes E2E
```

## Convencoes
- Toda logica de negocio via MediatR (Commands/Queries) — controllers sao thin
- React SPA servida diretamente pelo ASP.NET Core via `PhysicalFileProvider` (clientapp/)
- Rotas SPA: `/menu/{slug}`, `/cart`, `/checkout`, `/order/{slug}` — fallback para `index.html`
- CORS aberto (`SetIsOriginAllowed(_ => true)`) — portfolio sem restricao de origem
- `AddMenuNaMaoApplication()` + `AddMenuNaMaoInfrastructure()` nos extension methods
- `EnsureCreated` para dev; sem rate limiting (portfolio publico)

## Como Rodar
### Dev
```bash
# Subir dependencias
docker compose -f docker-compose.prod.yml up -d postgres rabbitmq

# API + Blazor admin
cd src/RVM.MenuNaMao.API
dotnet run

# React menu (em outro terminal)
cd src/menunomao-menu
npm install && npm run dev
```

### Testes
```bash
dotnet test test/RVM.MenuNaMao.Tests/
```

## Decisoes Arquiteturais
- **CQRS com MediatR**: separa leituras de escritas, facilita adicionar behaviors (validacao, cache) sem alterar handlers
- **React embarcado no ASP.NET Core**: simplifica deploy — um container serve admin e cardapio do cliente
- **RabbitMQ para pedidos**: pedidos em hora de pico nao bloqueiam a API; processamento e assíncrono e resiliente
- **GlobalExceptionMiddleware**: captura excecoes nao tratadas e retorna JSON padrao — sem stack trace para cliente
