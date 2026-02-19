import { client } from '@/shared/api';
import type { ChangePasswordRequest } from '../model/types';

export async function changePassword(payload: ChangePasswordRequest): Promise<void> {
  await client.put('/api/account-company/password', payload);
}