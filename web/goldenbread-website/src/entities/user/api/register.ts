import { client, type RegisterRequest } from '@/shared/api';

export async function register(credentials: RegisterRequest) {
  await client.post('/api/Auth/register/company', credentials);
}