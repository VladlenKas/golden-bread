import { client } from '@/shared/api';
import type { ProfileResponse } from '../model/types';

export async function getProfile(): Promise<ProfileResponse> {
  const { data } = await client.get('/api/account-company/profile');
  return data;
}