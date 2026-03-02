import { ref, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { getProductDetail, updateCartItem, toggleFavorite as toggleFavoriteApi } from './api';
import { ErrorKind } from '@/shared/api';
import { useNotifications } from '@/shared/composables';
import type { ProductDetail, UpdateCartItemRequest, AvailableBatch, CartSummary } from './types';

export function useProductDetail() {
  const route = useRoute();
  const router = useRouter();
  const { successToast, errorToast } = useNotifications();

  const productId = computed(() => Number(route.params.id));

  // Состояние
  const isLoading = ref(false);
  const isUpdating = ref(false);
  const product = ref<ProductDetail | null>(null);
  const selectedBatchId = ref<number | null>(null);
  const currentImageIndex = ref(0);

  // Вычисляемые свойства
  const currentBatch = computed(() => {
    if (!product.value?.availableBatches.length) return null;
    const targetId = selectedBatchId.value ?? product.value.currentBatchId;
    return product.value.availableBatches.find(b => b.productBatchId === targetId) 
        || product.value.availableBatches[0];
  });

  const quantity = computed(() => product.value?.quantityInCart ?? 0);
  const totalCost = computed(() => product.value?.totalCostInCart ?? 0);
  const hasInCart = computed(() => quantity.value > 0);

  const hasMultipleImages = computed(() => 
    (product.value?.imageUrls.length ?? 0) > 1
  );

  // Методы
  function changeImage(direction: 'next' | 'prev' | number) {
    const urls = product.value?.imageUrls;
    if (!urls?.length) return;
    const length = urls.length;
    if (typeof direction === 'number') {
      currentImageIndex.value = Math.max(0, Math.min(direction, length - 1));
    } else {
      const delta = direction === 'next' ? 1 : -1;
      currentImageIndex.value = (currentImageIndex.value + delta + length) % length;
    }
  }

  async function loadProduct() {
    try {
      isLoading.value = true;
      window.scrollTo({ top: 0, behavior: 'smooth' });
      
      const data = await getProductDetail(productId.value);
      product.value = data;
      selectedBatchId.value = data.currentBatchId || data.availableBatches[0]?.productBatchId || null;
      currentImageIndex.value = 0;
      
    } catch (error: any) {
      handleError(error);
      if (error.status === 404) router.push('/catalog');
    } finally {
      isLoading.value = false;
    }
  }

  function handleError(error: any) {
    if (error.kind === ErrorKind.Unknown || error.kind === ErrorKind.Network) {
      errorToast(error.message || 'Произошла ошибка');
    }
  }

  function updateLocalState(summary: CartSummary) {
    if (!product.value) return;
    product.value.quantityInCart = summary.totalQuantity;
    product.value.totalCostInCart = summary.totalCost;
    product.value.currentBatchId = summary.currentBatchId;
  }

  // Корзина: установить количество для выбранной партии
  async function setQuantity(newQuantity: number) {
    if (!product.value || !currentBatch.value) return;
    if (newQuantity < 0) return;

    try {
      isUpdating.value = true;

      const request: UpdateCartItemRequest = {
        productId: product.value.productId,
        productBatchId: currentBatch.value.productBatchId,
        quantity: newQuantity,
      };

      const summary = await updateCartItem(request); // Теперь возвращает CartSummary
      updateLocalState(summary);

    } catch (error: any) {
      handleError(error);
      throw error;
    } finally {
      isUpdating.value = false;
    }
  }

  const increment = () => setQuantity(quantity.value + 1);
  const decrement = () => setQuantity(quantity.value - 1);

  async function selectBatch(batch: AvailableBatch) {
    if (!product.value || batch.productBatchId === currentBatch.value?.productBatchId) return;

    selectedBatchId.value = batch.productBatchId;

    // Если в корзине уже есть товар — переносим на новую партию
    if (quantity.value > 0) {
      await setQuantity(quantity.value);
    }
  }

  async function toggleFavorite() {
    if (!product.value) return;
    try {
      isUpdating.value = true;
      await toggleFavoriteApi(product.value.productId);
      product.value.isFavorite = !product.value.isFavorite;
    } catch (error: any) {
      handleError(error);
    } finally {
      isUpdating.value = false;
    }
  }

  function goBack() {
    router.back();
  }

  return {
    // State
    product,
    isLoading,
    isUpdating,
    currentImageIndex,
    
    // Computed
    currentBatch,
    quantity,
    totalCost,
    hasInCart,
    hasMultipleImages,
    
    // Methods
    loadProduct,
    changeImage,
    setQuantity,
    increment,
    decrement,
    selectBatch,
    toggleFavorite,
    goBack,
  };
}