import { ref, computed } from 'vue';
import { createDeliveryInvoiceXlsx, getOrders } from './api';
import type { OrderListItemResponse, OrderStatus } from './types';

export function useOrders() {
  const orders = ref<OrderListItemResponse[]>([]);
  const isLoading = ref(false);
  const error = ref<string | null>(null);

  // Фильтры и сортировка
  const searchQuery = ref('');
  const statusFilter = ref<OrderStatus | 'all'>('all');
  const sortBy = ref<'newest' | 'oldest' | 'expensive' | 'cheap'>('newest');

  const fetchOrders = async () => {
    isLoading.value = true;
    error.value = null;
    
    try {
      const response = await getOrders();
      orders.value = response.listOrderItems;
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Ошибка загрузки заказов';
      console.error('Failed to fetch orders:', err);
    } finally {
      isLoading.value = false;
    }
  };

  // Отфильтрованные и отсортированные заказы
  const filteredOrders = computed(() => {
    let result = [...orders.value];

    // Фильтр по статусу
    if (statusFilter.value !== 'all') {
      result = result.filter(o => o.status === statusFilter.value);
    }

    // Поиск по номеру заказа
    if (searchQuery.value) {
      const query = searchQuery.value.toLowerCase();
      result = result.filter(o => 
        o.orderId.toString().includes(query)
      );
    }

    // Сортировка
    result.sort((a, b) => {
      switch (sortBy.value) {
        case 'newest':
          return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
        case 'oldest':
          return new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime();
        case 'expensive':
          return b.totalAmount - a.totalAmount;
        case 'cheap':
          return a.totalAmount - b.totalAmount;
        default:
          return 0;
      }
    });

    return result;
  });

  // Маппинг статусов (проверь какие статусы реально приходят с бэкенда!)
  const getStatusLabel = (status: OrderStatus): string => {
    const map: Record<OrderStatus, string> = {
      Awaiting: 'Ожидает подтверждения',
      AwaitingIngredients: 'Ожидает ингредиентов',
      InProgress: 'В процессе',
      Completed: 'Завершен',
      Canceled: 'Отменен'
    };
    return map[status] || status;
  };

  const getStatusClasses = (status: OrderStatus): string => {
    const map: Record<OrderStatus, string> = {
      Awaiting: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-500',
      AwaitingIngredients: 'bg-orange-100 text-orange-800 dark:bg-orange-900/30 dark:text-orange-500',
      InProgress: 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-500',
      Completed: 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-500',
      Canceled: 'bg-gray-100 text-gray-800 dark:bg-gray-800 dark:text-gray-400'
    };
    return map[status] || '';
  };

  // Опции для фильтра статусов
  const statusOptions: { value: OrderStatus | 'all'; label: string }[] = [
    { value: 'all', label: 'Все статусы' },
    { value: 'Awaiting', label: 'Ожидает подтверждения' },
    { value: 'AwaitingIngredients', label: 'Ожидает ингредиентов' },
    { value: 'InProgress', label: 'В процессе' },
    { value: 'Completed', label: 'Завершен' },
    { value: 'Canceled', label: 'Отменен' }
  ];

  // Форматирование
  const formatDate = (dateStr: string): string => {
    if (!dateStr) return '-';
    const date = new Date(dateStr);
    return date.toLocaleDateString('ru-RU', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    });
  };

  const formatPrice = (amount: number): string => {
    return new Intl.NumberFormat('ru-RU', {
      style: 'currency',
      currency: 'RUB',
      minimumFractionDigits: 0
    }).format(amount);
  };

  return {
    orders,
    isLoading,
    error,
    searchQuery,
    statusFilter,
    sortBy,
    filteredOrders,
    fetchOrders,
    getStatusLabel,
    getStatusClasses,
    statusOptions,
    formatDate,
    formatPrice
  };
}