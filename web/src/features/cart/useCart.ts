import { ref, computed } from 'vue';
import type { CartItem, CartSummary, CreateOrderResponse } from './types';
import {
  getCart,
  updateCartItem,
  toggleFavorite as toggleFavoriteApi,
  createOrder,
} from './api';
import { getLocalTimeZone, type DateValue } from '@internationalized/date';
import { format } from 'date-fns';
import { useNotifications } from '@/shared/composables';

export function useCart() {
  const { successToast } = useNotifications();
  const items = ref<CartItem[]>([]);
  const isLoading = ref(false);
  const isUpdating = ref(false);
  const isCreatingOrder = ref(false);
  const minimalDeliveryDate = ref<string | null>(null);
  const maximalDeliveryDate = ref<string | null>(null);
  const togglingFavorites = ref<Set<number>>(new Set());
  const PRODUCTION_LIMIT_MINUTES = 480;

  const loadCart = async () => {
    isLoading.value = true;
    try {
      const response = await getCart();
      minimalDeliveryDate.value = response.minimalDeliveryDate;
      maximalDeliveryDate.value = response.maximalDeliveryDate;

      items.value = (response.cartItemsList || []).map((dto) => ({
        ...dto,
        isSelected: true,
      }));
    } finally {
      isLoading.value = false;
    }
  };
  
const totalProductionTimeMinutes = computed(() => {
  const selectedItems = items.value.filter((item) => item.isSelected);
  return selectedItems.reduce((sum, item) => {
    const batchCount = item.quantityInCart;
    return sum + item.productionTimeMinutes * batchCount;
  }, 0);
});

const isWithinProductionLimit = computed(() => {
  return totalProductionTimeMinutes.value <= PRODUCTION_LIMIT_MINUTES;
});

const formattedProductionTime = computed(() => {
  const total = totalProductionTimeMinutes.value;
  const hours = Math.floor(total / 60);
  const minutes = total % 60;
  return hours > 0 && minutes > 0
    ? `${hours} ч ${minutes} мин`
    : hours > 0
      ? `${hours} ч`
      : `${minutes} мин`;
});

// Оставшееся время в процентах для прогресс-бара
const productionProgressPercent = computed(() => {
  return Math.min((totalProductionTimeMinutes.value / PRODUCTION_LIMIT_MINUTES) * 100, 100);
});

  // ✅ Верни CartSummary, а не CartSummaryResponse
  const summary = computed<CartSummary>(() => {
    const selectedItems = items.value.filter((item) => item.isSelected);
    return {
      totalItems: items.value.length,
      selectedItems: selectedItems.length,
      totalPrice: selectedItems.reduce(
        (sum, item) => sum + item.totalCostInCart,
        0,
      ),
      totalUnits: selectedItems.reduce(
        (sum, item) => sum + item.quantityPerBatch * item.quantityInCart,
        0,
      ),
    };
  });

  const updateQuantity = async (
    productId: number,
    productBatchId: number,
    newCount: number,
  ) => {
    const item = items.value.find((i) => i.productBatchId === productBatchId);
    if (!item || newCount < 0) return;

    const oldCount = item.quantityInCart;
    const oldCost = item.totalCostInCart;

    // Оптимистичное обновление UI
    if (newCount === 0) {
      items.value = items.value.filter(
        (i) => i.productBatchId !== productBatchId,
      );
    } else {
      item.quantityInCart = newCount;
      item.totalCostInCart = (item.totalCostInCart / oldCount) * newCount;
    }

    try {
      isUpdating.value = true;
      // Отправляем запрос, но не используем ответ напрямую (перезагрузим корзину)
      await updateCartItem({
        productId,
        productBatchId,
        quantity: newCount,
      });

      // Перезагружаем корзину для синхронизации с сервером
      await loadCart();
    } catch (error) {
      // Откат при ошибке
      if (newCount === 0) {
        await loadCart();
      } else {
        item.quantityInCart = oldCount;
        item.totalCostInCart = oldCost;
      }
      console.error('Failed to update cart:', error);
      throw error;
    } finally {
      isUpdating.value = false;
    }
  };

  const toggleFavorite = async (productId: number) => {
    const item = items.value.find((i) => i.productId === productId);
    if (!item) return;

    // Оптимистично меняем UI сразу
    const previousState = item.isFavorite;
    item.isFavorite = !previousState;

    // Добавляем в Set для возможного отображения загрузки на конкретной кнопке
    togglingFavorites.value.add(productId);

    try {
      await toggleFavoriteApi(productId);
      // Успех — ничего не делаем, UI уже обновлен
    } catch (error) {
      // ❌ Ошибка — откатываем изменения
      item.isFavorite = previousState;
      console.error('Failed to toggle favorite:', error);
      // Можно добавить toast уведомление об ошибке
      throw error;
    } finally {
      togglingFavorites.value.delete(productId);
    }
  };

  const incrementQuantity = async (
    productId: number,
    productBatchId: number,
  ) => {
    const item = items.value.find((i) => i.productBatchId === productBatchId);
    if (item) {
      await updateQuantity(productId, productBatchId, item.quantityInCart + 1);
    }
  };

  const decrementQuantity = async (
    productId: number,
    productBatchId: number,
  ) => {
    const item = items.value.find((i) => i.productBatchId === productBatchId);
    if (item && item.quantityInCart > 1) {
      await updateQuantity(productId, productBatchId, item.quantityInCart - 1);
    } else if (item && item.quantityInCart === 1) {
      await updateQuantity(productId, productBatchId, 0);
    }
  };

  const removeItem = async (productId: number, productBatchId: number) => {
    await updateQuantity(productId, productBatchId, 0);
  };

  const toggleSelection = (productBatchId: number) => {
    const item = items.value.find((i) => i.productBatchId === productBatchId);
    if (item) {
      item.isSelected = !item.isSelected;
    }
  };

  const selectAll = (selected: boolean) => {
    items.value.forEach((item) => {
      item.isSelected = selected;
    });
  };

  const isTogglingFavorite = (productId: number) =>
    togglingFavorites.value.has(productId);

  // ✅ Создание заказа с "искусственной" задержкой 3 секунды
  // useCart.ts
  const submitOrder = async (
    selectedDate: DateValue,
  ): Promise<CreateOrderResponse> => {
    isCreatingOrder.value = true;
    const startTime = Date.now();

    try {
      // Конвертируем DateValue в ISO строку прямо здесь
      const isoDate = format(
        selectedDate.toDate(getLocalTimeZone()),
        'yyyy-MM-dd',
      );

      const response = await createOrder({
        desiredDeliveryDate: isoDate,
      });

      // Задержка 3 секунды
      const elapsed = Date.now() - startTime;
      const remainingDelay = Math.max(0, 3000 - elapsed);

      if (remainingDelay > 0) {
        await new Promise((resolve) => setTimeout(resolve, remainingDelay));
      }

      // Очищаем выбранные товары
      items.value = items.value.filter((item) => !item.isSelected);

      return response;
    } finally {
      isCreatingOrder.value = false;
      successToast(
        `Поздравляем! Заказ оформлен успешно! Можете просмотреть детали в профиле`,
      );
    }
  };

  return {
    items,
    isLoading,
    isUpdating,
    summary,
    minimalDeliveryDate,
    maximalDeliveryDate,
    loadCart,
    updateQuantity,
    toggleSelection,
    removeItem,
    selectAll,
    incrementQuantity,
    decrementQuantity,
    toggleFavorite,
    isTogglingFavorite,
    isCreatingOrder,
    submitOrder, // Добавь сюда
    totalProductionTimeMinutes,
    isWithinProductionLimit,
    formattedProductionTime,
    productionProgressPercent 
  };
}
