/**
 * RVM.MenuNaMao — Gerador de Manual HTML
 *
 * Le os screenshots gerados pelo Playwright e produz um manual HTML standalone
 * com descritivos de cada funcionalidade.
 *
 * Uso:
 *   npx tsx docs/generate-html.ts
 *
 * Saida:
 *   docs/manual-usuario.html
 *   docs/manual-usuario.md
 */
import fs from 'fs';
import path from 'path';

const SCREENSHOTS_DIR = path.resolve(__dirname, 'screenshots');
const OUTPUT_HTML = path.resolve(__dirname, 'manual-usuario.html');
const OUTPUT_MD = path.resolve(__dirname, 'manual-usuario.md');

interface Section {
  id: string;
  title: string;
  description: string;
  screenshot: string;
  features: string[];
  tips?: string[];
}

const sections: Section[] = [
  // --- Painel Admin (Blazor) ---
  {
    id: 'home',
    title: '1. Pagina Inicial',
    description:
      'Tela de entrada do RVM.MenuNaMao. Apresenta o sistema de gestao de cardapio digital ' +
      'com acesso rapido ao painel administrativo e ao cardapio publico.',
    screenshot: '01-home',
    features: [
      'Visao geral do sistema',
      'Acesso ao painel administrativo',
      'Link para cardapio digital publico',
      'Identidade visual do produto',
    ],
  },
  {
    id: 'admin-dashboard',
    title: '2. Dashboard Administrativo',
    description:
      'Painel central do administrador com metricas em tempo real: pedidos do dia, ' +
      'receita, itens mais vendidos e status das mesas.',
    screenshot: '02-admin-dashboard',
    features: [
      'Total de pedidos do dia',
      'Receita acumulada',
      'Pedidos em andamento vs concluidos',
      'Itens mais pedidos',
      'Status das mesas (livres/ocupadas)',
    ],
    tips: [
      'O dashboard e atualizado automaticamente a cada 30 segundos.',
      'Clique nos cards de metricas para ir direto para o modulo correspondente.',
    ],
  },
  {
    id: 'admin-restaurantes',
    title: '3. Restaurantes',
    description:
      'Gerencie os restaurantes cadastrados no sistema. Cada restaurante tem seu ' +
      'proprio slug (URL amigavel), cardapio e QR Codes exclusivos.',
    screenshot: '03-admin-restaurantes',
    features: [
      'Listagem de restaurantes cadastrados',
      'Cadastrar novo restaurante com nome, slug e descricao',
      'Editar informacoes do restaurante',
      'Ativar/desativar restaurante',
      'Copiar URL do cardapio digital',
      'Gerar QR Code para impressao',
    ],
    tips: [
      'O slug define a URL do cardapio: /menu/{slug}. Escolha um nome curto e sem espacos.',
      'Restaurantes inativos nao aparecem no cardapio publico.',
    ],
  },
  {
    id: 'admin-cardapio',
    title: '4. Gestao do Cardapio',
    description:
      'Organize o cardapio do restaurante em categorias e itens. ' +
      'Defina precos, descricoes, fotos e disponibilidade de cada prato.',
    screenshot: '04-admin-cardapio',
    features: [
      'Criacao e edicao de categorias (Entradas, Pratos, Bebidas, Sobremesas)',
      'Adicionar itens com nome, descricao, preco e foto',
      'Ativar/desativar itens sem exclui-los',
      'Reordenar categorias e itens por arrastar e soltar',
      'Definir disponibilidade por horario',
    ],
    tips: [
      'Itens sem foto aparecem com imagem padrao no cardapio digital.',
      'Desativar um item o remove temporariamente do cardapio sem perder o cadastro.',
    ],
  },
  {
    id: 'admin-pedidos',
    title: '5. Pedidos',
    description:
      'Acompanhe todos os pedidos em tempo real. Visualize o status de cada pedido, ' +
      'os itens solicitados, a mesa e o valor total.',
    screenshot: '05-admin-pedidos',
    features: [
      'Listagem de pedidos com status (pendente, em preparo, pronto, entregue)',
      'Detalhes de cada pedido: itens, quantidades, observacoes',
      'Atualizar status do pedido',
      'Filtrar por status, mesa ou periodo',
      'Total do pedido e metodo de pagamento',
    ],
    tips: [
      'Pedidos novos aparecem no topo com destaque visual.',
      'Use os filtros para encontrar pedidos de uma mesa especifica.',
    ],
  },
  {
    id: 'admin-mesas',
    title: '6. Mesas e QR Codes',
    description:
      'Gerencie as mesas do restaurante e seus QR Codes. Cada mesa tem um QR Code ' +
      'unico que direciona o cliente para o cardapio digital ja identificado.',
    screenshot: '06-admin-mesas',
    features: [
      'Cadastro de mesas por numero/identificador',
      'Geracao e download de QR Code por mesa',
      'Status da mesa: livre, ocupada, reservada',
      'Historico de pedidos por mesa',
      'Imprimir QR Codes em lote',
    ],
    tips: [
      'Imprima os QR Codes e plastifique para uso nas mesas.',
      'O QR Code ja inclui o identificador da mesa — o cliente nao precisa digitar nada.',
    ],
  },
  {
    id: 'admin-estoque',
    title: '7. Controle de Estoque',
    description:
      'Monitore o estoque dos ingredientes e insumos. Receba alertas quando ' +
      'o estoque estiver baixo e registre entradas e saidas.',
    screenshot: '07-admin-estoque',
    features: [
      'Listagem de itens em estoque com quantidade atual',
      'Registrar entrada de insumos (compra/reposicao)',
      'Registrar saida manual (descarte, consumo)',
      'Alertas de estoque minimo',
      'Historico de movimentacoes',
    ],
    tips: [
      'Configure o nivel minimo de cada insumo para receber alertas antes de acabar.',
      'Conecte itens do cardapio aos insumos para desconto automatico no estoque ao receber pedidos.',
    ],
  },

  // --- Cardapio Digital (React) ---
  {
    id: 'menu-cardapio',
    title: '8. Cardapio Digital (Cliente)',
    description:
      'Interface publica que o cliente acessa pelo QR Code da mesa. ' +
      'Exibe todos os itens do cardapio com fotos, descricoes e precos. ' +
      'O cliente navega pelas categorias e adiciona itens ao carrinho.',
    screenshot: '08-menu-cardapio',
    features: [
      'Navegacao por categorias (Entradas, Pratos, Bebidas, Sobremesas)',
      'Foto, nome, descricao e preco de cada item',
      'Botao "Adicionar ao carrinho" com seletor de quantidade',
      'Busca por nome de item',
      'Indicador de itens no carrinho',
      'Totalmente responsivo para celular',
    ],
    tips: [
      'Toque em um item para ver a descricao completa e opcoes de personalizacao.',
      'O carrinho fica salvo mesmo se o cliente sair e voltar para o cardapio.',
    ],
  },
  {
    id: 'menu-carrinho',
    title: '9. Carrinho de Compras',
    description:
      'Resumo dos itens selecionados pelo cliente. Permite revisar quantidades, ' +
      'remover itens e adicionar observacoes antes de finalizar o pedido.',
    screenshot: '09-menu-carrinho',
    features: [
      'Lista de itens com quantidades e subtotais',
      'Alterar quantidade ou remover item',
      'Campo de observacoes por item',
      'Total do pedido calculado automaticamente',
      'Botao para continuar comprando',
      'Botao para ir ao checkout',
    ],
  },
  {
    id: 'menu-checkout',
    title: '10. Checkout',
    description:
      'Tela de confirmacao do pedido. O cliente informa seu nome, ' +
      'escolhe o metodo de pagamento e envia o pedido para a cozinha.',
    screenshot: '10-menu-checkout',
    features: [
      'Campo para nome do cliente',
      'Selecao de metodo de pagamento (dinheiro, cartao, Pix)',
      'Resumo final do pedido com valores',
      'Botao "Enviar Pedido" para confirmar',
      'Confirmacao visual apos envio',
    ],
    tips: [
      'Apos confirmar, o pedido aparece imediatamente no painel do admin.',
      'O cliente recebe uma tela de acompanhamento com o status do pedido.',
    ],
  },
  {
    id: 'menu-status-pedido',
    title: '11. Status do Pedido',
    description:
      'Apos confirmar o pedido, o cliente acompanha o status em tempo real: ' +
      'pendente, em preparo, pronto para retirar.',
    screenshot: '11-menu-status-pedido',
    features: [
      'Status atual do pedido com indicador visual',
      'Lista dos itens do pedido',
      'Estimativa de tempo (quando disponivel)',
      'Notificacao quando o pedido estiver pronto',
    ],
  },
];

// ---------------------------------------------------------------------------
// Gerar HTML
// ---------------------------------------------------------------------------
function imageToBase64(filePath: string): string | null {
  if (!fs.existsSync(filePath)) return null;
  const buffer = fs.readFileSync(filePath);
  return `data:image/png;base64,${buffer.toString('base64')}`;
}

function generateHTML(): string {
  const now = new Date().toLocaleDateString('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  });

  let sectionsHtml = '';
  for (const s of sections) {
    const desktopPath = path.join(SCREENSHOTS_DIR, `${s.screenshot}--desktop.png`);
    const mobilePath = path.join(SCREENSHOTS_DIR, `${s.screenshot}--mobile.png`);
    const desktopImg = imageToBase64(desktopPath);
    const mobileImg = imageToBase64(mobilePath);

    const featuresHtml = s.features.map((f) => `<li>${f}</li>`).join('\n            ');
    const tipsHtml = s.tips
      ? `<div class="tips">
          <strong>Dicas:</strong>
          <ul>${s.tips.map((t) => `<li>${t}</li>`).join('\n            ')}</ul>
        </div>`
      : '';

    const screenshotsHtml = desktopImg
      ? `<div class="screenshots">
          <div class="screenshot-group">
            <span class="badge">Desktop</span>
            <img src="${desktopImg}" alt="${s.title} - Desktop" />
          </div>
          ${
            mobileImg
              ? `<div class="screenshot-group mobile">
              <span class="badge">Mobile</span>
              <img src="${mobileImg}" alt="${s.title} - Mobile" />
            </div>`
              : ''
          }
        </div>`
      : '<p class="no-screenshot"><em>Screenshot nao disponivel. Execute o script Playwright para gerar.</em></p>';

    sectionsHtml += `
    <section id="${s.id}">
      <h2>${s.title}</h2>
      <p class="description">${s.description}</p>
      <div class="features">
        <strong>Funcionalidades:</strong>
        <ul>
            ${featuresHtml}
        </ul>
      </div>
      ${tipsHtml}
      ${screenshotsHtml}
    </section>`;
  }

  return `<!DOCTYPE html>
<html lang="pt-BR">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>RVM.MenuNaMao - Manual do Usuario</title>
  <style>
    :root {
      --primary: #f97316;
      --surface: #ffffff;
      --bg: #f4f6fa;
      --text: #1e293b;
      --text-muted: #64748b;
      --border: #e2e8f0;
      --sidebar-bg: #1c1917;
      --accent: #f97316;
    }
    * { box-sizing: border-box; margin: 0; padding: 0; }
    body {
      font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
      background: var(--bg);
      color: var(--text);
      line-height: 1.6;
    }
    .container { max-width: 1100px; margin: 0 auto; padding: 2rem 1.5rem; }
    header {
      background: var(--sidebar-bg);
      color: white;
      padding: 3rem 1.5rem;
      text-align: center;
    }
    header h1 { font-size: 2rem; margin-bottom: 0.5rem; }
    header p { color: #a8a29e; font-size: 1rem; }
    header .version { color: #78716c; font-size: 0.85rem; margin-top: 0.5rem; }
    nav {
      background: var(--surface);
      border-bottom: 1px solid var(--border);
      padding: 1rem 1.5rem;
      position: sticky;
      top: 0;
      z-index: 100;
    }
    nav .container { padding: 0; }
    nav ul { list-style: none; display: flex; flex-wrap: wrap; gap: 0.5rem; }
    nav a {
      display: inline-block;
      padding: 0.35rem 0.75rem;
      border-radius: 0.5rem;
      font-size: 0.85rem;
      color: var(--text);
      text-decoration: none;
      background: var(--bg);
      transition: background 0.2s;
    }
    nav a:hover { background: var(--primary); color: white; }
    section {
      background: var(--surface);
      border: 1px solid var(--border);
      border-radius: 1rem;
      padding: 2rem;
      margin-bottom: 2rem;
    }
    section h2 {
      font-size: 1.5rem;
      color: var(--primary);
      margin-bottom: 1rem;
      padding-bottom: 0.5rem;
      border-bottom: 2px solid var(--border);
    }
    .description { font-size: 1.05rem; margin-bottom: 1.25rem; color: var(--text); }
    .features, .tips {
      background: var(--bg);
      border-radius: 0.75rem;
      padding: 1rem 1.25rem;
      margin-bottom: 1.25rem;
    }
    .features ul, .tips ul { margin-top: 0.5rem; padding-left: 1.25rem; }
    .features li, .tips li { margin-bottom: 0.35rem; }
    .tips { background: #fff7ed; border-left: 4px solid var(--accent); }
    .tips strong { color: var(--accent); }
    .screenshots {
      display: flex;
      gap: 1.5rem;
      margin-top: 1rem;
      align-items: flex-start;
    }
    .screenshot-group {
      position: relative;
      flex: 1;
      border: 1px solid var(--border);
      border-radius: 0.75rem;
      overflow: hidden;
    }
    .screenshot-group.mobile { flex: 0 0 200px; max-width: 200px; }
    .screenshot-group img { width: 100%; display: block; }
    .badge {
      position: absolute;
      top: 0.5rem;
      right: 0.5rem;
      background: var(--sidebar-bg);
      color: white;
      font-size: 0.7rem;
      padding: 0.2rem 0.5rem;
      border-radius: 0.35rem;
      font-weight: 600;
      text-transform: uppercase;
    }
    .no-screenshot {
      background: var(--bg);
      padding: 2rem;
      border-radius: 0.75rem;
      text-align: center;
      color: var(--text-muted);
    }
    footer {
      text-align: center;
      padding: 2rem 1rem;
      color: var(--text-muted);
      font-size: 0.85rem;
    }
    @media (max-width: 768px) {
      .screenshots { flex-direction: column; }
      .screenshot-group.mobile { max-width: 100%; flex: 1; }
      section { padding: 1.25rem; }
    }
    @media print {
      nav { display: none; }
      section { break-inside: avoid; page-break-inside: avoid; }
      .screenshots { flex-direction: column; }
      .screenshot-group.mobile { max-width: 250px; }
    }
  </style>
</head>
<body>
  <header>
    <h1>RVM.MenuNaMao - Manual do Usuario</h1>
    <p>Cardapio Digital com QR Code — Guia Completo de Funcionalidades</p>
    <div class="version">Gerado em ${now} | RVM Tech</div>
  </header>

  <nav>
    <div class="container">
      <ul>
        ${sections.map((s) => `<li><a href="#${s.id}">${s.title}</a></li>`).join('\n        ')}
      </ul>
    </div>
  </nav>

  <div class="container">
    <section id="visao-geral">
      <h2>Visao Geral</h2>
      <p class="description">
        O <strong>RVM.MenuNaMao</strong> e um sistema de cardapio digital com QR Code para restaurantes.
        O cliente escaneia o QR Code da mesa, navega pelo cardapio, monta o pedido e envia diretamente
        para a cozinha — sem app instalado, sem garcom intermediario.
      </p>
      <div class="features">
        <strong>Recursos principais:</strong>
        <ul>
          <li><strong>Painel Admin Blazor</strong> — gestao completa de restaurantes, cardapio, pedidos e estoque</li>
          <li><strong>Cardapio Digital React</strong> — interface publica otimizada para celular via QR Code</li>
          <li><strong>Pedidos em tempo real</strong> — do cliente direto para a cozinha, sem intermediarios</li>
          <li><strong>QR Codes por mesa</strong> — cada mesa tem seu proprio QR Code com identificacao automatica</li>
          <li><strong>Multi-restaurante</strong> — gerencie varios restaurantes na mesma plataforma</li>
          <li><strong>Controle de estoque</strong> — integrado ao cardapio para baixa automatica de insumos</li>
        </ul>
      </div>
    </section>

    ${sectionsHtml}
  </div>

  <footer>
    <p>RVM Tech &mdash; Cardapio Digital com QR Code</p>
    <p>Documento gerado automaticamente com Playwright + TypeScript</p>
  </footer>
</body>
</html>`;
}

// ---------------------------------------------------------------------------
// Gerar Markdown
// ---------------------------------------------------------------------------
function generateMarkdown(): string {
  const now = new Date().toLocaleDateString('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  });

  let md = `# RVM.MenuNaMao - Manual do Usuario

> Cardapio Digital com QR Code — Guia Completo de Funcionalidades
>
> Gerado em ${now} | RVM Tech

---

## Visao Geral

O **RVM.MenuNaMao** e um sistema de cardapio digital com QR Code para restaurantes.
O cliente escaneia o QR Code da mesa, navega pelo cardapio, monta o pedido e envia diretamente
para a cozinha — sem app instalado, sem garcom intermediario.

**Recursos principais:**
- **Painel Admin Blazor** — gestao completa de restaurantes, cardapio, pedidos e estoque
- **Cardapio Digital React** — interface publica otimizada para celular via QR Code
- **Pedidos em tempo real** — do cliente direto para a cozinha, sem intermediarios
- **QR Codes por mesa** — cada mesa tem seu proprio QR Code com identificacao automatica
- **Multi-restaurante** — gerencie varios restaurantes na mesma plataforma
- **Controle de estoque** — integrado ao cardapio para baixa automatica de insumos

---

`;

  for (const s of sections) {
    const desktopExists = fs.existsSync(
      path.join(SCREENSHOTS_DIR, `${s.screenshot}--desktop.png`),
    );

    md += `## ${s.title}\n\n`;
    md += `${s.description}\n\n`;
    md += `**Funcionalidades:**\n`;
    for (const f of s.features) {
      md += `- ${f}\n`;
    }
    md += '\n';

    if (s.tips) {
      md += `> **Dicas:**\n`;
      for (const t of s.tips) {
        md += `> - ${t}\n`;
      }
      md += '\n';
    }

    if (desktopExists) {
      md += `| Desktop | Mobile |\n`;
      md += `|---------|--------|\n`;
      md += `| ![${s.title} - Desktop](screenshots/${s.screenshot}--desktop.png) | ![${s.title} - Mobile](screenshots/${s.screenshot}--mobile.png) |\n`;
    } else {
      md += `*Screenshot nao disponivel. Execute o script Playwright para gerar.*\n`;
    }
    md += '\n---\n\n';
  }

  md += `## Informacoes Tecnicas

| Item | Detalhe |
|------|---------|
| **Backend** | ASP.NET Core + Blazor Server |
| **Frontend** | React + React Router |
| **Banco de dados** | PostgreSQL 16 + EF Core |
| **QR Code** | Gerado server-side, download PNG/SVG |
| **Tempo real** | SignalR para atualizacao de pedidos |
| **Deploy** | Docker Compose + Nginx |

---

*Documento gerado automaticamente com Playwright + TypeScript — RVM Tech*
`;

  return md;
}

// ---------------------------------------------------------------------------
// Main
// ---------------------------------------------------------------------------
const html = generateHTML();
fs.writeFileSync(OUTPUT_HTML, html, 'utf-8');
console.log(`HTML gerado: ${OUTPUT_HTML}`);

const md = generateMarkdown();
fs.writeFileSync(OUTPUT_MD, md, 'utf-8');
console.log(`Markdown gerado: ${OUTPUT_MD}`);
