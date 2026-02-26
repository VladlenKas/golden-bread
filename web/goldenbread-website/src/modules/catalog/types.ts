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
  isFavourite: boolean
  quantityInCart: number
}

export interface UpdateCartItemRequest {
  productId: number,
  productBatchId: number,
  quantity: number
}


