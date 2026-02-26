import { client } from '@/shared/api';

import type { ProductListItem } from './types';

export async function getAllProducts(): Promise<ProductListItem[]> {
  const { data } = await client.get('/api/products');
  return data;
}

export async function addToFavorites(productId: number): Promise<void> {
  await client.post(`api/products/${productId}/favourite`);
}



