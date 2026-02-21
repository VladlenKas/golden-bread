import { client } from '@/shared/api';

import type { 
  AuthResponse, 
  LoginRequest, 
  RegisterRequest 
} from './types'

export async function register(value: RegisterRequest): Promise<void> {
  await client.post('/api/auth/register/company', value);
}

export async function login(value: LoginRequest): Promise<AuthResponse> {
  const { data } = await client.post('/api/auth/login/company', value);
  return data;
}

export async function logout(): Promise<void> {
  return await client.post('/api/auth/logout');
}

export async function me(): Promise<AuthResponse> {
  const { data } = await client.get('api/auth/me');
  return data;
}