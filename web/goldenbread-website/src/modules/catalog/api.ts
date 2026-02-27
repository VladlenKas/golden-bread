import { client } from '@/shared/api';

import type { CatalogResponse, ProductListItem, UpdateCartItemRequest } from './types';

export async function getAllProducts(): Promise<CatalogResponse> {
  const { data } = await client.get('/api/products');
  return data;
}

export async function toggleFavorite(productId: number): Promise<void> {
  await client.patch(`api/products/${productId}/favorite`);
}

export async function updateCartItem(request: UpdateCartItemRequest): Promise<number> {
  const { data } = await client.patch(`api/products/${request.productId}/cart`, request);
  return data;
}