import { client } from '@/shared/api';

import type { CartSummary, CatalogResponse, ProductDetail, ProductListItem, UpdateCartItemRequest } from './types';

export async function getAllProducts(): Promise<CatalogResponse> {
  const { data } = await client.get('/api/products');
  return data;
}

export async function toggleFavorite(productId: number): Promise<void> {
  await client.patch(`api/products/${productId}/favorite`);
}

export async function updateCartItem(request: UpdateCartItemRequest): Promise<CartSummary> {
  const { data } = await client.patch(`api/products/${request.productId}/update-cart`, request);
  return data;
}

export async function getProductDetail(productId: number): Promise<ProductDetail> {
  const { data } = await client.get(`/api/products/${productId}`);
  return data;
} 