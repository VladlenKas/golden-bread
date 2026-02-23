export interface ProductListItem {
  productId: number;
  name: string;
  description: string;
  productionTime: number;
  categoryId: number;
  categoryName: string;
  categoryColor: string;
  quantityPerBatch: number;
  salePrice: number;
  imageUrl: string | null;
}

export interface ProductCard {
  productId: number;
  name: string;
  description: string;
  salePrice: number;
  productionTime: number;
  quantityPerBatch: number;
  imageUrl: string | null;
  categoryName: string;
  categoryColor: string;
}

