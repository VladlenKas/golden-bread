// Данные профиля

export interface UpdateRequisitesRequest {
  name: string;
  inn: string;
  ogrn: string;
}

export interface UpdateContactsRequest {
  phone: string;
  address: string;
}

export interface ChangePasswordRequest {
  oldPassword: string;
  newPassword: string;
}

export interface ChangeEmailRequest {
  newEmail: string;
  password: string;
}

export interface ProfileResponse {
  email: string;
  name: string;
  inn: string;
  ogrn: string;
  phone: string;
  address: string;
  orders: unknown[]; 
}

// Данные по заказу
export type OrderStatus = 'Awaiting' | 'AwaitingIngredients' | 'InProgress' | 'Completed' | 'Canceled';

export interface OrderListItemResponse {
  orderId: number;
  startDate: string; 
  endDate: string;  
  createdAt: string; 
  status: OrderStatus;
  quantityOrderItems: number;
  totalAmount: number;
}

export interface OrdersListResponse {
  listOrderItems: OrderListItemResponse[];
}