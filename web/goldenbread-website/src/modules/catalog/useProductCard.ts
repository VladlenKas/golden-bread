import { ref } from 'vue';

export function useProductCard() {
  const isAddingToCart = ref(false);
  const isAddingToFavorites = ref(false);

  async function addToCart(productId: number) {
    // TODO: Реализовать добавление в корзину
    console.log('Add to cart:', productId);
  }

  async function addToFavorites(productId: number) {
    // TODO: Реализовать добавление в избранное
    console.log('Add to favorites:', productId);
  }

  return {
    isAddingToCart,
    isAddingToFavorites,
    addToCart,
    addToFavorites,
  };
}