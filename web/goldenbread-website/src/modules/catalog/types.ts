export interface CatalogResponse {
  productsList: ProductListItem[];
  categories: Category[];
}

export interface ProductListItem {
  productId: number;
  name: string;
  description: string;
  productionTime: number;
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
  salePrice: number;
  weight: number;
  productionTime: number;
  categoryName: string;
  categoryColor: string;
  productBatchId: number;
  quantityPerBatch: number;
  availableBatches: AvailableBatch[];
  imageUrls: string[];
  isFavorite: boolean;
  quantityInCart: number;
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
  salePrice: number;
}

export interface UpdateCartItemRequest {
  productId: number,
  productBatchId: number,
  quantity: number
}


