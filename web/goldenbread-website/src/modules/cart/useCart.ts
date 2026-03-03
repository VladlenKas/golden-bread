// modules/cart/composables/useCart.ts
import { ref, computed } from 'vue';
import type { CartItem, CartSummary } from './types';

// Тестовые данные
const mockCartItems: CartItem[] = [
  {
    cartItemId: '1',
    productId: '101',
    productName: 'Профитроли с ванильным кремом',
    description: 'Нежные заварные пирожные с воздушным ванильным кремом и шоколадной глазурью',
    imageUrl: 'profitroli.jpg',
    categoryName: 'Десерты',
    categoryColor: 'f472b6',
    
    batchId: 'b1',
    quantityPerBatch: 8,
    unitPrice: 45.50,
    totalBatchPrice: 364.00,
    
    batchCount: 2,
    totalUnits: 16,
    totalPrice: 728.00,
    
    productionTimeMinutes: 120,
    isSelected: true,
  },
  {
    cartItemId: '2',
    productId: '102',
    productName: 'Круассаны классические',
    description: 'Слоеное тесто с масляной прослойкой, хрустящие и ароматные',
    imageUrl: 'cherry_pie_6.jpg',
    categoryName: 'Выпечка',
    categoryColor: 'fbbf24',
    
    batchId: 'b2',
    quantityPerBatch: 12,
    unitPrice: 38.00,
    totalBatchPrice: 456.00,
    
    batchCount: 1,
    totalUnits: 12,
    totalPrice: 456.00,
    
    productionTimeMinutes: 180,
    isSelected: true,
  },
  {
    cartItemId: '3',
    productId: '103',
    productName: 'Макаруны ассорти',
    description: 'Набор французских миндальных пирожных в трех вкусах: малина, фисташка, шоколад',
    imageUrl: null,
    categoryName: 'Десерты',
    categoryColor: 'f472b6',
    
    batchId: 'b3',
    quantityPerBatch: 6,
    unitPrice: 85.00,
    totalBatchPrice: 510.00,
    
    batchCount: 3,
    totalUnits: 18,
    totalPrice: 1530.00,
    
    productionTimeMinutes: 240,
    isSelected: false,
  },
  {
    cartItemId: '4',
    productId: '104',
    productName: 'Хлеб бородинский',
    description: 'Классический ржаной хлеб с кориандром и солодом',
    imageUrl: 'bread.jpg',
    categoryName: 'Хлеб',
    categoryColor: 'a78bfa',
    
    batchId: 'b4',
    quantityPerBatch: 1,
    unitPrice: 120.00,
    totalBatchPrice: 120.00,
    
    batchCount: 5,
    totalUnits: 5,
    totalPrice: 600.00,
    
    productionTimeMinutes: 90,
    isSelected: true,
  },
];

export function useCart() {
  const items = ref<CartItem[]>(mockCartItems);
  const isLoading = ref(false);

  const summary = computed<CartSummary>(() => {
    const selectedItems = items.value.filter(item => item.isSelected);
    return {
      totalItems: items.value.length,
      selectedItems: selectedItems.length,
      totalPrice: selectedItems.reduce((sum, item) => sum + item.totalPrice, 0),
      totalUnits: selectedItems.reduce((sum, item) => sum + item.totalUnits, 0),
    };
  });

  const updateBatchCount = (cartItemId: string, newCount: number) => {
    const item = items.value.find(i => i.cartItemId === cartItemId);
    if (item && newCount >= 1) {
      item.batchCount = newCount;
      item.totalUnits = item.quantityPerBatch * newCount;
      item.totalPrice = item.totalBatchPrice * newCount;
    }
  };

  const toggleSelection = (cartItemId: string) => {
    const item = items.value.find(i => i.cartItemId === cartItemId);
    if (item) {
      item.isSelected = !item.isSelected;
    }
  };

  const removeItem = (cartItemId: string) => {
    items.value = items.value.filter(i => i.cartItemId !== cartItemId);
  };

  const selectAll = (selected: boolean) => {
    items.value.forEach(item => {
      item.isSelected = selected;
    });
  };

  const incrementBatch = (cartItemId: string) => {
    const item = items.value.find(i => i.cartItemId === cartItemId);
    if (item) {
      updateBatchCount(cartItemId, item.batchCount + 1);
    }
  };

  const decrementBatch = (cartItemId: string) => {
    const item = items.value.find(i => i.cartItemId === cartItemId);
    if (item && item.batchCount > 1) {
      updateBatchCount(cartItemId, item.batchCount - 1);
    }
  };

  return {
    items,
    isLoading,
    summary,
    updateBatchCount,
    toggleSelection,
    removeItem,
    selectAll,
    incrementBatch,
    decrementBatch,
  };
}