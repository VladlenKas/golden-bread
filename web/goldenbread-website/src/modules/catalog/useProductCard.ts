import { ref, watch } from 'vue';
import { updateCartItem, toggleFavorite } from './api'
import { ErrorKind } from '@/shared/api';
import { useNotifications } from '@/shared/composables';
import type { ProductListItem, UpdateCartItemRequest } from './types';
import { router } from '@/app/providers/router';

export function useProductCard(props: ProductListItem) {
  // Переиспользуемая логика
  const { unhandledErrorToast } = useNotifications();

  // Локальное состояние
  const isLoading = ref(false);
  const isFavorite = ref(props.isFavorite);
  const quantityInCart = ref(props.quantityInCart);

  const incrementCart = () => updateCartQuantity(quantityInCart.value + 1);
  const decrementCart = () => updateCartQuantity(quantityInCart.value - 1);
  const goToProductDetail = () => router.push(`/product/${props.productId}`);

  async function updateCartQuantity(newQuantity: number) {
    if (newQuantity < 0) return;
    
    try {
      isLoading.value = true;
      const request: UpdateCartItemRequest = {
        productId: props.productId,
        productBatchId: props.productBatchId,
        quantity: newQuantity,
      };
      quantityInCart.value = (await updateCartItem(request)).totalQuantity;
      props.quantityInCart = quantityInCart.value
    } catch (error: any) {
      if (error.kind === ErrorKind.Unknown)
        unhandledErrorToast(error.message, error.status);
      throw error;
    } finally {
      isLoading.value = false;
    }
  }

  async function toggleFavoriteStatus() {
    try {
      isLoading.value = true;
      await toggleFavorite(props.productId);
      isFavorite.value = !isFavorite.value;
    } catch (error: any) {
      if (error.kind === ErrorKind.Unknown)
        unhandledErrorToast(error.message, error.status);
      throw error;
    } finally {
      isLoading.value = false;
    }
  }

  return {
    // Состояние
    isLoading,
    isFavorite,
    quantityInCart,

    // Действия
    incrementCart,
    decrementCart,
    toggleFavoriteStatus,
    goToProductDetail,
  };
}
