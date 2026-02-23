import { client } from '@/shared/api';

import type {
  ChangeEmailRequest,
  ChangePasswordRequest,
  ProfileResponse,
  UpdateContactsRequest,
  UpdateRequisitesRequest,
} from './types';

export async function changeEmail(value: ChangeEmailRequest): Promise<void> {
  await client.put('/api/company-profile/email', value);
}

export async function changePassword(value: ChangePasswordRequest,): Promise<void> {
  await client.put('/api/company-profile/password', value);
}

export async function updateContacts(value: UpdateContactsRequest,): Promise<void> {
  await client.put('/api/company-profile/contacts', value);
}

export async function updateRequisites(value: UpdateRequisitesRequest,): Promise<void> {
  await client.put('/api/company-profile/requisites', value);
}

export async function getProfile(): Promise<ProfileResponse> {
  const { data } = await client.get('/api/company-profile');
  return data;
}
