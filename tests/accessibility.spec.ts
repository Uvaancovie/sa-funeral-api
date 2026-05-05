import { test, expect, Page } from '@playwright/test';
import AxeBuilder from '@axe-core/playwright';

const mockProducts = [
  {
    productId: 1,
    id: 'oxford-casket',
    name: 'Oxford Casket',
    category: 'casket',
    description: 'Premium casket',
    price: null,
    priceOnRequest: true,
    images: '[]',
    colorVariations: null,
    specifications: null,
    features: '["Cherry","Dome"]',
    inStock: true,
    featured: false,
    createdAt: new Date().toISOString(),
    updatedAt: null
  }
];

async function stubProducts(page: Page) {
  await page.route('**/api/products', async route => {
    await route.fulfill({
      status: 200,
      contentType: 'application/json',
      body: JSON.stringify(mockProducts)
    });
  });
}

test('catalog page passes accessibility checks', async ({ page }) => {
  await stubProducts(page);
  await page.goto('/#/catalog');

  await expect(page.getByRole('heading', { name: 'Product Expo' })).toBeVisible();
  const results = await new AxeBuilder({ page }).analyze();
  expect(results.violations).toEqual([]);
});
