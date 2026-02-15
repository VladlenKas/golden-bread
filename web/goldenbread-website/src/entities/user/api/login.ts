import { client, type AuthResponse } from '@/shared/api';
import type { LoginCredentials } from '../model/types';

export async function login(credentials: LoginCredentials): Promise<AuthResponse> {
  const { data } = await client.post('/api/Auth/login/company', credentials);
  return data;
}