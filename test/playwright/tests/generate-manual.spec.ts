/**
 * RVM.MenuNaMao — Gerador de Manual Visual
 *
 * Playwright script que navega por todas as telas do sistema,
 * captura screenshots em diferentes estados e viewports, e gera as imagens
 * para o manual do usuario.
 *
 * Uso:
 *   cd test/playwright
 *   npx playwright test tests/generate-manual.spec.ts --reporter=list
 */
import { test, type Page } from '@playwright/test';
import path from 'path';
import fs from 'fs';

const BASE_URL = process.env.MENUNAMAO_BASE_URL ?? 'https://menunamao.lab.rvmtech.com.br';
const MENU_BASE_URL = process.env.MENUNAMAO_MENU_URL ?? 'https://menunamao.lab.rvmtech.com.br';
const SCREENSHOTS_DIR = path.resolve(__dirname, '../../../docs/screenshots');

// Garantir que o diretorio de screenshots existe
if (!fs.existsSync(SCREENSHOTS_DIR)) {
  fs.mkdirSync(SCREENSHOTS_DIR, { recursive: true });
}

/** Captura desktop (1280x800) + mobile (390x844) */
async function capture(page: Page, name: string, opts?: { fullPage?: boolean }) {
  const fullPage = opts?.fullPage ?? true;
  await page.screenshot({
    path: path.join(SCREENSHOTS_DIR, `${name}--desktop.png`),
    fullPage,
  });
  await page.setViewportSize({ width: 390, height: 844 });
  await page.screenshot({
    path: path.join(SCREENSHOTS_DIR, `${name}--mobile.png`),
    fullPage,
  });
  await page.setViewportSize({ width: 1280, height: 800 });
}

// ---------------------------------------------------------------------------
// 1. Painel Admin (Blazor)
// ---------------------------------------------------------------------------
test.describe('1. Painel Administrativo', () => {
  test('1.1 Home / Landing', async ({ page }) => {
    await page.goto(`${BASE_URL}/`);
    await page.waitForLoadState('networkidle');
    await capture(page, '01-home');
  });

  test('1.2 Dashboard Admin', async ({ page }) => {
    await page.goto(`${BASE_URL}/admin`);
    await page.waitForLoadState('networkidle');
    await capture(page, '02-admin-dashboard');
  });

  test('1.3 Restaurantes', async ({ page }) => {
    await page.goto(`${BASE_URL}/admin/restaurants`);
    await page.waitForLoadState('networkidle');
    await capture(page, '03-admin-restaurantes');
  });

  test('1.4 Cardapio (Categorias e Itens)', async ({ page }) => {
    await page.goto(`${BASE_URL}/admin/menu`);
    await page.waitForLoadState('networkidle');
    await capture(page, '04-admin-cardapio');
  });

  test('1.5 Pedidos', async ({ page }) => {
    await page.goto(`${BASE_URL}/admin/orders`);
    await page.waitForLoadState('networkidle');
    await capture(page, '05-admin-pedidos');
  });

  test('1.6 Mesas', async ({ page }) => {
    await page.goto(`${BASE_URL}/admin/tables`);
    await page.waitForLoadState('networkidle');
    await capture(page, '06-admin-mesas');
  });

  test('1.7 Estoque', async ({ page }) => {
    await page.goto(`${BASE_URL}/admin/stock`);
    await page.waitForLoadState('networkidle');
    await capture(page, '07-admin-estoque');
  });
});

// ---------------------------------------------------------------------------
// 2. Cardapio Digital (React)
// ---------------------------------------------------------------------------
test.describe('2. Cardapio Digital (Cliente)', () => {
  test('2.1 Cardapio Digital — pagina do restaurante', async ({ page }) => {
    await page.goto(`${MENU_BASE_URL}/menu/demo`);
    await page.waitForLoadState('networkidle');
    await capture(page, '08-menu-cardapio');
  });

  test('2.2 Carrinho de Compras', async ({ page }) => {
    await page.goto(`${MENU_BASE_URL}/cart`);
    await page.waitForLoadState('networkidle');
    await capture(page, '09-menu-carrinho');
  });

  test('2.3 Checkout', async ({ page }) => {
    await page.goto(`${MENU_BASE_URL}/checkout`);
    await page.waitForLoadState('networkidle');
    await capture(page, '10-menu-checkout');
  });

  test('2.4 Status do Pedido', async ({ page }) => {
    await page.goto(`${MENU_BASE_URL}/order/demo`);
    await page.waitForLoadState('networkidle');
    await capture(page, '11-menu-status-pedido');
  });
});
