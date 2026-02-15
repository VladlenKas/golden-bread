import axios from 'axios';
import { useNotifications } from '@/shared/composables/useNotifications';
import { ErrorKind, type ApiError } from './types/error';

export const client = axios.create({
  baseURL: 'https://localhost:7107',
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true,
});

client.interceptors.response.use(
  (response) => response,
  (error) => {
    const { errorApiToast, errorToast } = useNotifications();

    if (!error.response) {
      const err: ApiError = {
        message: "Не удалось установить соединение с сервером",
        kind: ErrorKind.Network,
        status: 0,
        data: null
      };
      errorApiToast(err.message, 0);
      return Promise.reject(err);
    }

    const status = error.response.status;
    const message = error.response.message;

    switch (status) {
      case 401: {
        errorToast("Пользователь не найден или не существует");
        break;
      }
      case 422: {
        errorToast("Введенны невалидные данные. Проверьте правильность заполнения полей");
        break;
      }
      case 500: {
        errorApiToast("Внутренняя ошибка на сервере", 500);
        break;
      }
      default:
        errorApiToast(message, status);
    }

    return Promise.reject({
      message,
      kind: ErrorKind.Http,
      status,
      data: error.response.data
    } as ApiError);
  },
);
