// modules/cart/types.ts
export interface CartItem {
  cartItemId: string;
  productId: string;
  productName: string;
  description: string;
  imageUrl: string | null;
  categoryName: string;
  categoryColor: string;
  
  // Информация о выбранной партии
  batchId: string;
  quantityPerBatch: number;
  unitPrice: number;
  totalBatchPrice: number;
  
  // Количество партий
  batchCount: number;
  
  // Итоговые значения
  totalUnits: number; // quantityPerBatch * batchCount
  totalPrice: number; // totalBatchPrice * batchCount
  
  // Дополнительно
  productionTimeMinutes: number;
  isSelected: boolean;
  isFavorite: boolean;
}

export interface CartSummary {
  totalItems: number;
  selectedItems: number;
  totalPrice: number;
  totalUnits: number;
}