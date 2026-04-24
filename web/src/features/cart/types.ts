export interface CartDto {
  cartItemsList: ProductCartItemDto[] | null;
  minimalDeliveryDate: string | null; 
  maximalDeliveryDate: string | null;
}

export interface ProductCartItemDto {
  productId: number;
  name: string;
  productionTimeMinutes: number;
  productBatchId: number;
  quantityPerBatch: number;
  imageUrl: string | null;
  isFavorite: boolean;
  IsSelected: boolean,
  quantityInCart: number;
  totalCostInCart: number;
}

// Для фронта 
export interface CartItem extends ProductCartItemDto {
  isSelected: boolean; // Только для фронта (чекбоксы)
}

export interface UpdateCartItemRequest {
  productId: number;
  productBatchId: number;
  quantity: number;
}

export interface CartSummary {
  totalItems: number;
  selectedItems: number;
  totalPrice: number;
  totalUnits: number;
}

export interface CartSummaryResponse {
  totalQuantity: number;
  totalCost: number;
  currentBatchId: number;
}

export interface CreateOrderRequest {
  desiredDeliveryDate: string; // ISO format: "2026-04-15"
}

export interface CreateOrderResponse {
  success: boolean;
  orderId: number;
}