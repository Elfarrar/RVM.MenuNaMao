import { expect, test } from '@playwright/test';

const defaultBaseUrl = process.env.MENUNAMAO_BASE_URL ?? 'https://menunamao.lab.rvmtech.com.br';

test.describe('Restaurant API', () => {
  test.skip(
    process.env.MENUNAMAO_RUN_SMOKE !== '1',
    'Defina MENUNAMAO_RUN_SMOKE=1 para rodar o smoke contra um ambiente real.',
  );

  test('GET /api/menu/{slug} — menu publico por slug retorna 200 ou 404', async ({ request, baseURL }) => {
    const currentBaseUrl = baseURL ?? defaultBaseUrl;
    const response = await request.get(`${currentBaseUrl}/api/menu/demo`);
    expect([200, 404]).toContain(response.status());
  });

  test('GET /api/tables/resolve — sem token retorna 400 ou 401', async ({ request, baseURL }) => {
    const currentBaseUrl = baseURL ?? defaultBaseUrl;
    const response = await request.get(`${currentBaseUrl}/api/tables/resolve`);
    expect([400, 401, 422]).toContain(response.status());
  });

  test('GET /api/orders/{id} — id invalido retorna 400, 401 ou 404', async ({ request, baseURL }) => {
    const currentBaseUrl = baseURL ?? defaultBaseUrl;
    const fakeId = '00000000-0000-0000-0000-000000000000';
    const response = await request.get(`${currentBaseUrl}/api/orders/${fakeId}`);
    expect([400, 401, 404]).toContain(response.status());
  });

  test('GET /api/admin/restaurants — requer autenticacao (401)', async ({ request, baseURL }) => {
    const currentBaseUrl = baseURL ?? defaultBaseUrl;
    const response = await request.get(`${currentBaseUrl}/api/admin/restaurants`);
    expect([401, 403]).toContain(response.status());
  });

  test('GET /api/admin/restaurants/{id}/dashboard — requer autenticacao (401)', async ({ request, baseURL }) => {
    const currentBaseUrl = baseURL ?? defaultBaseUrl;
    const fakeId = '00000000-0000-0000-0000-000000000000';
    const response = await request.get(`${currentBaseUrl}/api/admin/restaurants/${fakeId}/dashboard`);
    expect([401, 403, 404]).toContain(response.status());
  });

  test('POST /api/orders — sem body retorna 400 ou 401', async ({ request, baseURL }) => {
    const currentBaseUrl = baseURL ?? defaultBaseUrl;
    const response = await request.post(`${currentBaseUrl}/api/orders`, { data: {} });
    expect([400, 401, 422]).toContain(response.status());
  });
});
