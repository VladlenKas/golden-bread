import { client } from '@/shared/api';

import type { CartResponse, UpdateCartItemRequest } from './types';

export async function getAllProducts(): Promise<CartResponse> {
  const { data } = await client.get('/api/products');
  return data;
}

export async function updateCartItem(request: UpdateCartItemRequest): Promise<number> {
  const { data } = await client.patch(`api/products/${request.productId}/update-cart`, request);
  return data;
}