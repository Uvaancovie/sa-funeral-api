import { test, expect } from '@playwright/test';

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
  },
  {
    productId: 2,
    id: 'lincoln-dome',
    name: 'Lincoln Dome',
    category: 'casket',
    description: 'Classic design',
    price: null,
    priceOnRequest: true,
    images: '[]',
    colorVariations: null,
    specifications: null,
    features: '["White","Dome"]',
    inStock: true,
    featured: false,
    createdAt: new Date().toISOString(),
    updatedAt: null
  }
];

async function stubProducts(page: import('@playwright/test').Page) {
  await page.route('**/api/products', async route => {
    await route.fulfill({
      status: 200,
      contentType: 'application/json',
      body: JSON.stringify(mockProducts)
    });
  });
}

test('catalog shows products', async ({ page }) => {
  await stubProducts(page);
  await page.goto('/#/catalog');

  await expect(page.getByRole('heading', { name: 'Product Expo' })).toBeVisible();

  const cards = page.locator('[data-testid="catalog-card"]');
  await expect(cards).toHaveCount(mockProducts.length);
});

test('product card opens detail page', async ({ page }) => {
  await stubProducts(page);
  await page.goto('/#/catalog');

  const firstCard = page.locator('[data-testid="catalog-card"]').first();
  const productName = (await firstCard.locator('h2').first().textContent())?.trim();

  await firstCard.click();

  await expect(page).toHaveURL(/#\/product\/.+/);
  const title = page.locator('[data-testid="product-title"]');
  await expect(title).toBeVisible();
  if (productName) {
    await expect(title).toHaveText(productName);
  }
});
