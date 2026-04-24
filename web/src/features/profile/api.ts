import { client, clientDocument } from '@/shared/api';

import type {
  ChangeEmailRequest,
  ChangePasswordRequest,
  ProfileResponse,
  UpdateContactsRequest,
  UpdateRequisitesRequest,
  OrdersListResponse
} from './types';
import type { ProductListItem } from '../catalog/types';

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

export async function getOrders(): Promise<OrdersListResponse> {
  const { data } = await client.get('/api/orders');
  return data;
}

export async function getFavorites(): Promise<ProductListItem[]> {
  const { data } = await client.get('/api/favorites');
  return data;
}

export async function createDeliveryInvoiceXlsx(orderId: number) {
  const response = await clientDocument.post(
    `/api/document/delivery-invoice-xlsx/${orderId}`,
    {}, 
    { 
      responseType: 'blob', 
      headers: {
        'Accept': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
      }
    }
  );
  
  return response; 
}
