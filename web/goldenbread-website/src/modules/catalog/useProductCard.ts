import { ref } from 'vue';
import { updateCartItem, toggleFavorite } from './api'
import { ErrorKind } from '@/shared/api';
import { useNotifications } from '@/shared/composables';
import type { UpdateCartItemRequest } from './types';
import { router } from '@/app/providers/router';

export function useProductCard() {
  const { unhandledErrorToast, successToast } = useNotifications();
  const isLoading = ref(false);

  async function updateCartQuantity(
    productId: number, 
    productBatchId: number, 
    quantity: number,
  ) {
    try {
      isLoading.value = true
      const request: UpdateCartItemRequest = {
        productId: productId,
        productBatchId: productBatchId,
        quantity: quantity,
      }
      quantity = await updateCartItem(request);
      return quantity;
    } catch (error: any) {
      if (error.kind === ErrorKind.Unknown)
        unhandledErrorToast(error.message, error.status);
      throw error;
    } finally {
      isLoading.value = false;
    }
  }

  async function switchFavoriteStatus(productId: number, isFavorite: boolean):
    Promise<boolean> {
    try {
      isLoading.value = true
      await toggleFavorite(productId);
      return !isFavorite;
    } catch (error: any) {
      if (error.kind === ErrorKind.Unknown)
        unhandledErrorToast(error.message, error.status);
      throw error;
    } finally {
      isLoading.value = false;
    }
  }

  function goToProductDetail(productId: number) {
    sessionStorage.setItem('catalogScroll', String(window.scrollY));
    router.push(`/product/${productId}`);
  }

  return {
    updateCartQuantity,
    switchFavoriteStatus,
    goToProductDetail,
    isLoading
  };
}
