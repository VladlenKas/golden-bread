import axios from 'axios';
import { useNotifications } from '@/shared/composables/useNotifications';
import { ErrorKind } from './types';
import { API_BASE_URL } from '../constants';

export interface ApiError {
  message: string;
  kind: ErrorKind;
  status: number;
  data: [] | null;
}

export const client = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true,
});

client.interceptors.response.use(
  (response) => response,
  (error) => {
    const { unhandledErrorToast, errorToast, infoToast } = useNotifications();

    if (!error.response) {
      const err: ApiError = {
        message: "Не удалось установить соединение с сервером",
        kind: ErrorKind.Network,
        status: 0,
        data: null
      };

      errorToast(err.message);
      return Promise.reject(err);
    }

    let message;
    let kind = ErrorKind.Http;
    const status = error.response.status;
    const type = error.response.data.type;
    
    if (error.response.data.message) {
      message = error.response.data.message;
    } else {
      message = error.message;
    }

    switch (status) {
      case 401: {
        if (type === "SessionExpiredException") {
          infoToast(message);
        } else {
          errorToast(message);
        }
        break;
      }
      case 409: {
        errorToast("Введены данные, которые в настоящий момент уже используются. Попробуйте ввести другие");
        break;
      }
      case 404: {
        errorToast("У вас недостаточно прав для этого действия. Обновите страницу и попробуйте снова")
        kind = ErrorKind.NoRights;
        break;
      }
      case 422: {
        errorToast("Введенны невалидные данные. Проверьте правильность заполнения полей");
        break;
      }
      case 500: {
        errorToast("Произошла внутренняя ошибка на сервере");
        break;
      }
      default:
        unhandledErrorToast(message, status);
        kind = ErrorKind.Unknown
    }

    return Promise.reject({
      message,
      kind: kind,
      status,
      data: error.response.data
    } as ApiError);
  },
);
