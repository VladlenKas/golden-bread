import { client } from '@/shared/api';

import type { CartDto, CartSummaryResponse, UpdateCartItemRequest, CreateOrderRequest, CreateOrderResponse } from './types';

export async function getCart(): Promise<CartDto> {
  const { data } = await client.get('/api/cart');
  return data;
}

export async function updateCartItem(request: UpdateCartItemRequest): Promise<CartSummaryResponse> {
  const { data } = await client.put(`/api/cart/${request.productId}`, request);
  return data;
}

export async function toggleFavorite(productId: number): Promise<void> {
  await client.patch(`/api/favorites/${productId}`);
}

export async function toggleSelection(productId: number): Promise<void> {
  await client.patch(`/api/cart/${productId}/selection`);
}

export const createOrder = async (data: CreateOrderRequest): Promise<CreateOrderResponse> => {
  const response = await client.post<CreateOrderResponse>('api/orders', data);
  return response.data;
};
