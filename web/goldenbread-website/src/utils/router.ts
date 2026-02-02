import { createRouter, createWebHistory } from 'vue-router';
import { useLoginStore } from '@/features/login/loginStore';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      redirect: '/home',
    },
    {
      path: '/home',
      name: 'home',
      component: () => import('../pages/HomePage.vue'),
    },
    {
      path: '/profile',
      name: 'profile',
      component: () => import('../pages/ProfilePage.vue'),
      meta: { requiresAuth: true}
    },
  ],
});

router.beforeEach((to, from, next ) => {
  const loginStore = useLoginStore();
  
  if (to.meta.requestAuth && !loginStore.isAuthenticated) {
    next({name: 'Home'});
  } else {
    next();
  }
})

export default router;
