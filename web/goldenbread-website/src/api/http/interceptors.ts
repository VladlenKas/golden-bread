import { useNotifications } from '@/composables/useNotifications';
import { createApiError } from '../http/types';
import { useLoginStore } from '@/features/login/loginStore';
import { AxiosError } from 'axios';
import { MESSAGES } from '@/utils/messages';
import { api } from './api';

api.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    const authStore = useLoginStore();
    const { errorApiToast } = useNotifications();

    console.log('Ловим ошибку');
    if (!error.response) {
      errorApiToast(MESSAGES.NETWORK_ERROR, 0);
      return Promise.reject(new Error(MESSAGES.NETWORK_ERROR));
    }

    const { status } = error.response;

    console.log('Ловим другую ошибку', status);
    switch (status) {
      case 401: {
        authStore.logout();
        errorApiToast(MESSAGES.UNAUTHORIZED, 401);
        break;
      }
      case 403: {
        errorApiToast(MESSAGES.FORBIDDEN, 403);
        break;
      }
      case 404: {
        errorApiToast(MESSAGES.NOT_FOUND, 404);
        break;
      }
      case 500: {
        errorApiToast(MESSAGES.SERVER_ERROR, 500);
        break;
      }
      default: {
        const apiError = createApiError(error);
        return Promise.reject(apiError);
      }
    }
  },
);
