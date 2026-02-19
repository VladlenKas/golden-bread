import { client } from '@/shared/api';
import type { UpdateContactsRequest } from '../model/types';

export async function updateContacts(payload: UpdateContactsRequest): Promise<void> {
  await client.put('/api/account-company/contacts', payload);
}