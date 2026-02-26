import { ref } from 'vue';
import { addToFavorites as addToFavoritesApi } from './api'
import { ErrorKind } from '@/shared/api';

export function useProductCard() {
  const isAddingToCart = ref(false);
  const isAddingToFavorites = ref(false);

  async function addToCart(productId: number) {
    // TODO: Реализовать добавление в корзину
    console.log('Add to cart:', productId);
  }
  
  async function addToFavorites(productId: number, isFav: boolean): Promise<boolean> {
  try {
    await addToFavoritesApi(productId);
    return !isFav;  
  } catch (error: any) {
    if (error.kind === ErrorKind.Unknown) 
      unhandledErrorToast(error.message, error.status);
    throw error;
  }
}

  return {
    isAddingToCart,
    isAddingToFavorites,
    addToCart,
    addToFavorites,
  };
}

function unhandledErrorToast(message: any, status: any) {
  throw new Error('Function not implemented.');
}
