import { client } from '@/shared/api';

import type { CartSummary, CatalogResponse, ProductDetail, ProductListItem, UpdateCartItemRequest } from './types';

export async function getAllProducts(): Promise<CatalogResponse> {
  const { data } = await client.get('/api/catalog');
  return data;
}

export async function getProductDetail(productId: number): Promise<ProductDetail> {
  const { data } = await client.get(`/api/catalog/${productId}`);
  return data;
}

export async function toggleFavorite(productId: number): Promise<void> {
  await client.patch(`api/favorites/${productId}`);
}

export async function updateCartItem(request: UpdateCartItemRequest): Promise<CartSummary> {
  const { data } = await client.put(`api/cart/${request.productId}`, request);
  return data;
}