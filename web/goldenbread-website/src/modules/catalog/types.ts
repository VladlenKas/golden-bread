export interface CatalogResponse {
  productsList: ProductListItem[];
  categories: Category[];
}

export interface ProductListItem {
  productId: number;
  name: string;
  description: string;
  productionTimeMinutes: number;
  categoryId: number;
  categoryName: string;
  categoryColor: string;
  productBatchId: number;
  quantityPerBatch: number;
  salePrice: number;
  imageUrl: string | null;
  isFavorite: boolean
  quantityInCart: number
}

export interface Category {
  productCategoryId: number; 
  name: string;
  color: string;
  productCount: number;
}

export interface ProductDetail {
  productId: number;
  categoryId: number;
  name: string;
  description: string;
  weight: number;
  productionTimeMinutes: number;
  shelfLifeDays: number;
  storageTempMin: number;
  storageTempMax: number
  categoryName: string;
  categoryColor: string;
  currentBatchId: number;
  availableBatches: AvailableBatch[];
  imageUrls: string[];
  isFavorite: boolean;
  quantityInCart: number;
  totalCostInCart: number;
  ingredients: Ingredient[];
}

export interface Ingredient {
  ingredientId: number;
  name: string;
  quantity: number;
  unit: string;
}     

export interface AvailableBatch {
  productBatchId: number;
  quantityPerBatch: number;                   
  unitPrice: number;
  totalPrice: number;
}

export interface CartSummary {
  totalQuantity: number;
  totalCost: number;
  currentBatchId: number;
}

export interface UpdateCartItemRequest {
  productId: number,
  productBatchId: number,
  quantity: number
}


