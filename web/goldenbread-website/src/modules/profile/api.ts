import { client } from '@/shared/api';

import type {
  ChangeEmailRequest,
  ChangePasswordRequest,
  ProfileResponse,
  UpdateContactsRequest,
  UpdateRequisitesRequest,
} from './types';

export async function changeEmail(value: ChangeEmailRequest): Promise<void> {
  await client.put('/api/account-company/email', value);
}

export async function changePassword(value: ChangePasswordRequest,): Promise<void> {
  await client.put('/api/account-company/password', value);
}

export async function updateContacts(value: UpdateContactsRequest,): Promise<void> {
  await client.put('/api/account-company/contacts', value);
}

export async function updateRequisites(value: UpdateRequisitesRequest,): Promise<void> {
  await client.put('/api/account-company/requisites', value);
}

export async function getProfile(): Promise<ProfileResponse> {
  const { data } = await client.get('/api/account-company/profile');
  return data;
}
