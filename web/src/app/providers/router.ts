import { useAuthStore } from '@/features/auth/stores';
import { createRouter, createWebHistory } from 'vue-router';

export const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      redirect: '/home',
    },
    {
      path: '/home',
      name: 'home',
      component: () => import('@/pages/HomePage.vue'),
    },
    {
      path: '/cart',
      name: 'cart',
      component: () => import('@/features/cart/CartPage.vue'),
    },
    {
      path: '/catalog',
      name: 'catalog',
      component: () => import('@/features/catalog/CatalogPage.vue'),
    },
    {
      path: '/product/:id',
      name: 'ProductDetail',
      component: () => import('@/features/catalog/ProductDetailPage.vue'),
    },
    {
      path: '/auth',
      name: 'auth',
      component: () => import('@/features/auth/AuthPage.vue'),
      meta: { guestOnly: true },
    },
    {
      path: '/profile',
      name: 'profile',
      component: () => import('@/features/profile/ProfilePage.vue'),
      meta: { requiresAuth: true },
    },
  ],
});

router.beforeEach(async (to) => {
  const authStore = useAuthStore();

  if (!authStore.isInitialized) {
    await authStore.me();
  }

  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    return '/auth';
  }

  if (to.meta.guestOnly && authStore.isAuthenticated) {
    return '/profile';
  }
});
