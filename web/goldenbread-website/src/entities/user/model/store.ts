import { defineStore } from 'pinia';
import { me } from '../api/me';
import { login as loginApi } from '../api/login'
import { register as registerApi } from '../api/register'
import { 
  VerificationStatus, 
  type AuthResponse, 
  type RegisterRequest} from '@/shared/api';
import type { LoginCredentials } from './types';


export const useUserStore = defineStore('user', {
  state: () => ({
    user: null as AuthResponse | null,
    isAuthenticated: false,
    isInitialized: false,
  }),

  actions: {
    setUser(user: AuthResponse) {
      this.user = user;
      this.isAuthenticated = true;
    },

    clearUser() {
      this.user = null;
      this.isAuthenticated = false;
    },

    async initialize() {
      if (this.isInitialized) return;

      try {
        const user = await me();
        if (user.verificationStatus === VerificationStatus.Approved) {
          this.setUser(user);
        }
      } catch {
        this.clearUser();
      } finally {
        this.isInitialized = true;
      }
    },

    async login(credentials: LoginCredentials): Promise<AuthResponse> {
      const response = await loginApi(credentials);

      if (response.verificationStatus === VerificationStatus.Approved) {
        this.setUser(response);
      }

      return response;
    },

    async register(credentials: RegisterRequest) {
      await registerApi(credentials);
    },

    logout() {
      this.clearUser();
    },
  },
});
