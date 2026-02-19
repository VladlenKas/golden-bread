import { client } from '@/shared/api';
import type { UpdateRequisitesRequest } from '../model/types';

export async function updateRequisites(payload: UpdateRequisitesRequest): Promise<void> {
  await client.put('/api/account-company/requisites', payload);
}