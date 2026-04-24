import { ref, computed } from 'vue';
import { getFavorites } from './api';
import type { ProductListItem } from '../catalog/types';
import { router } from '@/app/providers/router';

export function useFavorites(props: ProductListItem) {
  const favorites = ref<ProductListItem[]>([]);
  const isLoading = ref(false);
  const sortBy = ref<'newest' | 'price-asc' | 'price-desc'>('newest');

  const goToProductDetail = () => router.push(`/product/${props.productId}`);
  
  const fetchFavorites = async () => {
    isLoading.value = true;
    try {
      const response = await getFavorites();
      favorites.value = response; // или response.listItems если обёртка
    } finally {
      isLoading.value = false;
    }
  };

  const sortedFavorites = computed(() => {
    const list = [...favorites.value];
    switch (sortBy.value) {
      case 'price-asc':
        return list.sort((a, b) => a.salePrice - b.salePrice);
      case 'price-desc':
        return list.sort((a, b) => b.salePrice - a.salePrice);
      case 'newest':
      default:
        // Предполагаем, что новые в конце списка или есть поле addedAt
        return list;
    }
  });

  return {
    favorites,
    isLoading,
    sortBy,
    sortedFavorites,
    fetchFavorites,
    goToProductDetail
  };
}