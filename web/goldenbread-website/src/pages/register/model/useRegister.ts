// pages/register/model/useRegister.ts
import { useNotifications } from '@/shared/composables';
import { useUserStore } from '@/entities/user';
import { ErrorKind, type RegisterRequest } from '@/shared/api/';

export function useRegister() {
  const userStore = useUserStore();
  const {
    successToast,
    errorApiToast,
  } = useNotifications();

  async function handleRegister(credentials: RegisterRequest, setErrors: any) {
    try {
      await userStore.register(credentials);
      successToast("Регистрация прошла успешно. На вашу электронную почту поступит уведомление," +
         "когда аккаунт будет проверен");
    } catch (error: any) {
      if (error.status === 422 || error.status === 409) {
        const apiErrors = error.data.errors || [];

        const groupedErrors = apiErrors.reduce((acc: any, err: any) => {
          acc[err.propertyName] = err.errorMessage;
          return acc;
        }, {});

        setErrors({
          name: groupedErrors.Name,
          email: groupedErrors.Email,
          password: groupedErrors.Password,
          inn: groupedErrors.Inn,
          ogrn: groupedErrors.Ogrn,
        });
      } else if (error.kind != ErrorKind.Network) {
        errorApiToast(error.message, error.status);
      }
    }
  }

  return { handleRegister };
}
