import { createRouter, createWebHistory } from 'vue-router';
import { useUserStore } from '@/entities/user';

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
      component: () => import('@/pages/home/ui/HomePage.vue'),
    },
    {
      path: '/login',
      name: 'login',
      component: () => import('@/pages/login/ui/LoginPage.vue'),
      meta: { guestOnly: true },  
    },
    {
      path: '/register',
      name: 'register',
      component: () => import('@/pages/register/ui/RegisterPage.vue'),
      meta: { guestOnly: true },  
    },
    {
      path: '/profile',
      name: 'profile',
      component: () => import('@/pages/profile/ui/ProfilePage.vue'),
      meta: { requiresAuth: true }
    },
  ],
});

router.beforeEach( async (to) => {
  const userStore = useUserStore();
  await userStore.initialize();
  
  if (to.meta.requiresAuth && !userStore.isAuthenticated) {
    return { name: 'login', query: { redirect: to.fullPath } };
  }

  if (to.meta.guestOnly && userStore.isAuthenticated) {
    return { name: 'profile' };
  }
})
