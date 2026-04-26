# Testes — RVM.MenuNaMao

## Testes Unitarios
- **Framework:** xUnit + Moq
- **Localizacao:** `test/RVM.MenuNaMao.Tests/`
- **Total:** 61 testes
- **Foco:** handlers MediatR (Commands/Queries), pipeline behaviors, logica de Order, geracao de QR Code

```bash
dotnet test test/RVM.MenuNaMao.Tests/
```

## Testes E2E (Playwright)
- **Localizacao:** `test/playwright/`
- **Cobertura:**
  - Admin Blazor: gerenciar categorias, itens, visualizar pedidos
  - React menu: navegar cardapio, adicionar ao carrinho, checkout

```bash
cd test/playwright
npm install
npx playwright install --with-deps
npx playwright test
```

Variaveis de ambiente necessarias:
```
MENUNAMAO_BASE_URL=http://localhost:5000
```

## CI
- **Arquivo:** `.github/workflows/ci.yml`
- Pipeline: build → testes unitarios → Playwright
- RabbitMQ mockado em testes unitarios; testes E2E usam instancia real via Docker
