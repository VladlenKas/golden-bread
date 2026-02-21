import axios from 'axios';
import { useNotifications } from '@/shared/composables/useNotifications';
import { ErrorKind } from './types';

export interface ApiError {
  message: string;
  kind: ErrorKind;
  status: number;
  data: [] | null;
}

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
    const { unhandledErrorToast, errorToast, infoToast } = useNotifications();

    if (!error.response) {
      const err: ApiError = {
        message: "Не удалось установить соединение с сервером",
        kind: ErrorKind.Network,
        status: 0,
        data: null
      };
      unhandledErrorToast(err.message, 0);
      return Promise.reject(err);
    }

    const status = error.response.status;
    const message = error.response.data.message;
    const type = error.response.data.type;
    let kind = ErrorKind.Http;

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
        errorToast("У вас недостаточно прав для этого действия. Выполните вход в систему и попробуйте снова")
        kind = ErrorKind.NoRights;
        break;
      }
      case 422: {
        errorToast("Введенны невалидные данные. Проверьте правильность заполнения полей");
        break;
      }
      case 500: {
        unhandledErrorToast("Внутренняя ошибка на сервере", 500);
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
