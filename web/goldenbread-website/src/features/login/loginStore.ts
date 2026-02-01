import type { LoginRequest, LoginResponse } from '@/api/login/loginDto';
import { loginService } from '@/api/login/loginService';
import { defineStore } from 'pinia';
import { useCookies } from 'vue3-cookies';

const { cookies } = useCookies();

export const useLoginStore = defineStore('login', {
  state: () => ({
    user: null as LoginResponse | null,
    isAuthenticated: false,
  }),

  actions: {
    async login(request: LoginRequest) {
      const response = await loginService(request);

      this.user = response;
      this.isAuthenticated = true;

      cookies.set(
        'session',
        response.session,
        new Date(response.sessionExpiresAt),
      );
    },

    logout() {
      cookies.remove('session');
      this.user = null;
      this.isAuthenticated = false;
    },
  },
});
