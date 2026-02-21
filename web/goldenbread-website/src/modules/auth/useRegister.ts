import { useFormMappingErros, useNotifications } from "@/shared/composables";
import type { RegisterRequest } from "./types";
import { ErrorKind } from "@/shared/api";
import { register } from "./api";
import { router } from "@/app/providers/router";

export function useRegister() {
  const { mapErrors } = useFormMappingErros(); 
  const { successToast, unhandledErrorToast } = useNotifications();

  async function handleRegister(values: RegisterRequest, setErrors: any) {
    try {
      await register(values);
      successToast("Регистрация прошла успешно. На вашу электронную почту поступит уведомление, когда аккаунт будет проверен");
      router.push('/login')
    } catch (error: any) {
      if (error.status === 422 || 409) {
        const fieldMapping = { Name: 'name', Inn: 'inn', Ogrn: 'ogrn', Email: 'email', Password: 'password' }
        setErrors(mapErrors(error, fieldMapping));
      } else if (error.kind === ErrorKind.Unknown) {
        unhandledErrorToast(error.message, error.status);
      }
    }
  }

  return { handleRegister };
}