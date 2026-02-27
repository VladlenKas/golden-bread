import { ref, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { getProductDetail, updateCartItem, toggleFavorite } from './api';
import { ErrorKind } from '@/shared/api';
import { useNotifications } from '@/shared/composables';
import type { ProductDetail, UpdateCartItemRequest, AvailableBatch } from './types';

export function useProductDetail() {
  const route = useRoute();
  const router = useRouter();
  const { unhandledErrorToast, successToast } = useNotifications();
  
  const product = ref<ProductDetail | null>(null);
  const isLoading = ref(false);
  const isActionLoading = ref(false);
  const selectedBatch = ref<AvailableBatch | null>(null);
  const currentImageIndex = ref(0);

  const productId = computed(() => Number(route.params.id));

  const currentBatch = computed(() => {
    return selectedBatch.value || product.value?.availableBatches[0] || null;
  });

  const totalPrice = computed(() => {
    if (!currentBatch.value) return 0;
    return currentBatch.value.salePrice * currentBatch.value.quantityPerBatch;
  });

  const unitPrice = computed(() => {
    return currentBatch.value?.salePrice || 0;
  });

  const hasMultipleImages = computed(() => {
    return (product.value?.imageUrls.length ?? 0) > 1;
  });

  const isFavorite = computed(() => product.value?.isFavorite ?? false);
  
  const quantityInCart = computed(() => product.value?.quantityInCart ?? 0);

  async function loadProduct() {
    try {
      isLoading.value = true;
       window.scrollTo({ top: 0, behavior: 'smooth' });
      const data = await getProductDetail(productId.value);
      product.value = data;
      const defaultBatch = data.availableBatches.find(b => b.productBatchId === data.productBatchId) 
        || data.availableBatches[0];
      selectedBatch.value = defaultBatch || null;
    } catch (error: any) {
      if (error.kind === ErrorKind.Unknown) {
        unhandledErrorToast(error.message, error.status);
      }
      if (error.status === 404) {
        router.push('/catalog');
      }
    } finally {
      isLoading.value = false;
    }
  }

  async function updateCart(quantity: number) {
    if (!product.value || !currentBatch.value) return;
    
    try {
      isActionLoading.value = true;
      const request: UpdateCartItemRequest = {
        productId: product.value.productId,
        productBatchId: currentBatch.value.productBatchId,
        quantity: quantity,
      };
      const newQuantity = await updateCartItem(request);
      if (product.value) {
        product.value.quantityInCart = newQuantity;
      }
      
    } catch (error: any) {
      if (error.kind === ErrorKind.Unknown) {
        unhandledErrorToast(error.message, error.status);
      }
    } finally {
      isActionLoading.value = false;
    }
  }

  async function addToCart() {
    await updateCart(quantityInCart.value + 1);
  }

  async function removeFromCart() {
    const newQty = Math.max(0, quantityInCart.value - 1);
    await updateCart(newQty);
  }

  async function toggleFavoriteStatus() {
    if (!product.value) return;
    
    try {
      isActionLoading.value = true;
      await toggleFavorite(product.value.productId);
      product.value.isFavorite = !product.value.isFavorite;
    } catch (error: any) {
      if (error.kind === ErrorKind.Unknown) {
        unhandledErrorToast(error.message, error.status);
      }
    } finally {
      isActionLoading.value = false;
    }
  }

  function selectBatch(batch: AvailableBatch) {
    selectedBatch.value = batch;
  }

  function nextImage() {
    if (!product.value?.imageUrls.length) return;
    currentImageIndex.value = (currentImageIndex.value + 1) % product.value.imageUrls.length;
  }

  function prevImage() {
    if (!product.value?.imageUrls.length) return;
    currentImageIndex.value = currentImageIndex.value === 0 
      ? product.value.imageUrls.length - 1 
      : currentImageIndex.value - 1;
  }

  function goToImage(index: number) {
    currentImageIndex.value = index;
  }

  function goBack() {
    router.back();
  }

  return {
    product,
    isLoading,
    isActionLoading,
    currentImageIndex,
    currentBatch,
    totalPrice,
    unitPrice,
    hasMultipleImages,
    isFavorite,
    quantityInCart,
    loadProduct,
    addToCart,
    removeFromCart,
    updateCartQuantity: updateCart,
    toggleFavoriteStatus,
    selectBatch,
    nextImage,
    prevImage,
    goToImage,
    goBack,
  };
}