import type { RegisterRequest } from '@/api/register/registerDto';
import { registerService } from '@/api/register/registerService';
import { defineStore } from 'pinia';

export const useRegisterStore = defineStore('register', {
  actions: {
    async register(request: RegisterRequest) {
      return await registerService(request);
    },
  },
});
