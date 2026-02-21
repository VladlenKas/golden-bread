import { useAuthStore } from '@/modules/auth/stores';
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
      path: '/about',
      name: 'about',
      component: () => import('@/pages/AboutPage.vue'),
    },
    {
      path: '/contacts',
      name: 'contacts',
      component: () => import('@/pages/ContactsPage.vue'),
    },
    {
      path: '/cart',
      name: 'cart',
      component: () => import('@/modules/cart/CartPage.vue'),
    },
    {
      path: '/catalog',
      name: 'catalog',
      component: () => import('@/modules/catalog/CatalogPage.vue'),
    },
    {
      path: '/login',
      name: 'login',
      component: () => import('@/modules/auth/LoginPage.vue'),
      meta: { guestOnly: true },
    },
    {
      path: '/register',
      name: 'register',
      component: () => import('@/modules/auth/RegisterPage.vue'),
      meta: { guestOnly: true },
    },
    {
      path: '/profile',
      name: 'profile',
      component: () => import('@/modules/profile/ProfilePage.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/privacy',
      name: 'privacy',
      component: () => import('@/pages/PrivacyPage.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/faq',
      name: 'faq',
      component: () => import('@/pages/FaqPage.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/docs',
      name: 'docs',
      component: () => import('@/pages/DocsPage.vue'),
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
    return '/login';
  }

  if (to.meta.guestOnly && authStore.isAuthenticated) {
    return '/profile';
  }
});
