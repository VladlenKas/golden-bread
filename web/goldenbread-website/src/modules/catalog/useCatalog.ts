import { ref, computed, onMounted, nextTick } from 'vue';
import { useNotifications } from '@/shared/composables';
import { ErrorKind } from '@/shared/api';
import type { Category, ProductListItem } from './types';
import { getAllProducts } from './api';

// Опции времени приготовления
const timeOptions = [
  { label: 'Быстро (до 60 мин)', value: 'fast', max: 60 },
  { label: 'Средне (60-120 мин)', value: 'medium', min: 60, max: 120 },
  { label: 'Долго (более 120 мин)', value: 'slow', min: 120 },
];

export function useCatalog() {
  const { unhandledErrorToast } = useNotifications();

  const products = ref<ProductListItem[]>([]);
  const categories = ref<Category[]>([]);
  const isLoading = ref(false);

  const selectedCategory = ref<number | null>(null);
  const searchQuery = ref('');
  const priceRange = ref<[number, number]>([0, 1000]);
  const selectedProductionTime = ref<string[]>([]);
  const sortBy = ref('name');

  onMounted(() => {
    fetchProducts();
    restoreScrollPosition();
  });

  function restoreScrollPosition() {
    const saved = sessionStorage.getItem('catalogScroll');
    if (saved) {
      nextTick(() => {
        window.scrollTo(0, parseInt(saved));
        sessionStorage.removeItem('catalogScroll');
      });
    }
  }

  // Загрузка продукций
  async function fetchProducts() {
    isLoading.value = true;
    try {
      const response = await getAllProducts();
      products.value = response.productsList;
      categories.value = response.categories;
    } catch (error: any) {
      if (error.kind === ErrorKind.Unknown)
        unhandledErrorToast(error.message, error.status);
    } finally {
      isLoading.value = false;
    }
  }

  // Фильтрация
  const filteredProducts = computed(() => {
    let result = products.value;

    // Фильтр по категории
    if (selectedCategory.value) {
      result = result.filter((p) => p.categoryId === selectedCategory.value);
    }

    // Поиск по названию
    if (searchQuery.value) {
      const query = searchQuery.value.toLowerCase();
      result = result.filter(
        (p) =>
          p.name.toLowerCase().includes(query) ||
          p.description.toLowerCase().includes(query),
      );
    }

    // Фильтр по цене
    result = result.filter(
      (p) =>
        p.salePrice >= priceRange.value[0] &&
        p.salePrice <= priceRange.value[1],
    );

    // Фильтр по времени приготовления
    if (selectedProductionTime.value.length > 0) {
      result = result.filter((p) => {
        return selectedProductionTime.value.some((time) => {
          const option = timeOptions.find((t) => t.value === time);
          if (!option) return false;
          if (option.max && !option.min) return p.productionTime <= option.max;
          if (option.min && option.max)
            return (
              p.productionTime >= option.min && p.productionTime <= option.max
            );
          if (option.min && !option.max) return p.productionTime >= option.min;
          return false;
        });
      });
    }

    // Сортировка
    result = [...result].sort((a, b) => {
      switch (sortBy.value) {
        case 'name':
          return a.name.localeCompare(b.name);
        case 'price-asc':
          return a.salePrice - b.salePrice;
        case 'price-desc':
          return b.salePrice - a.salePrice;
        case 'time':
          return a.productionTime - b.productionTime;
        default:
          return 0;
      }
    });

    return result;
  });

  // Фильтр по времени приготовления (для checkbox)
  function toggleTimeFilter(value: string, checked: boolean) {
    if (checked) {
      selectedProductionTime.value.push(value);
    } else {
      selectedProductionTime.value = selectedProductionTime.value.filter(
        (v) => v !== value,
      );
    }
  }

  // Группировка по категориям
  const groupedProducts = computed(() => {
    const groups = new Map();

    filteredProducts.value.forEach((product) => {
      if (!groups.has(product.categoryId)) {
        groups.set(product.categoryId, {
          category: {
            id: product.categoryId,
            name: product.categoryName,
            color: product.categoryColor,
          },
          items: [],
        });
      }
      groups.get(product.categoryId).items.push(product);
    });

    if (selectedCategory.value) {
      const selected = groups.get(selectedCategory.value);
      return selected ? [selected] : [];
    }

    return Array.from(groups.values());
  });

  // Счетчик активных фильтров
  const activeFiltersCount = computed(() => {
    let count = 0;
    if (selectedCategory.value) count++;
    if (searchQuery.value) count++;
    if (priceRange.value[0] > 0 || priceRange.value[1] < 1000) count++;
    if (selectedProductionTime.value.length > 0) count++;
    return count;
  });

  // Сброс фильтров
  function resetFilters() {
    selectedCategory.value = null;
    searchQuery.value = '';
    priceRange.value = [0, 1000];
    selectedProductionTime.value = [];
    sortBy.value = 'name';
  }

  return {
    // Данные
    products,
    isLoading,
    categories,
    filteredProducts,
    groupedProducts,

    // Фильтры
    selectedCategory,
    searchQuery,
    priceRange,
    selectedProductionTime,
    sortBy,
    timeOptions,

    // Вычисляемые
    activeFiltersCount,
    restoreScrollPosition,

    // Методы
    fetchProducts,
    resetFilters,
    toggleTimeFilter,
  };
}
