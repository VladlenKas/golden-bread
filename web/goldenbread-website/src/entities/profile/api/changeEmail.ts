import { client } from '@/shared/api';
import type { ChangeEmailRequest } from '../model/types';

export async function changeEmail(payload: ChangeEmailRequest): Promise<void> {
  await client.put('/api/account-company/email', payload);
} 