import { ref } from 'vue';
import { defineStore } from 'pinia';
import { me as meApi } from './api'
import { VerificationStatus, type AuthResponse } from './types';

export const useAuthStore = defineStore('auth', () => {
  const isAuthenticated = ref(false);
  const isInitialized = ref(false); 

  function setAuthenticated(value: boolean) {
    isAuthenticated.value = value;
  }

  function setInitialized(value: boolean) {
    isInitialized.value = value;
  }

  async function me() {
    isAuthenticated.value = false;
    
    try {
      const account = await meApi();

      if (account != null && account.verificationStatus === VerificationStatus.Approved) {
        isAuthenticated.value = true;
      }
    } catch (error) {
      isAuthenticated.value = false;
    } finally {
      isInitialized.value = true;
    }
  }

  function logout() {
    isAuthenticated.value = false;
  }

  return {
    isAuthenticated,
    isInitialized,
    setAuthenticated,
    setInitialized,
    me,
    logout,
  };
});

