import { useRegisterStore } from '@/features/register/registerStore';
import { useNotifications } from '@/composables/useNotifications';
import { ref } from 'vue';

export function useRegisterDialog() {
  const isOpen = ref(false);
  const registerStore = useRegisterStore();
  const { successRegisterToast, failedRegisterToast } = useNotifications();

  async function onSubmit(values: any, setErrors: any) {
    try {
      await registerStore.register(values);
      successRegisterToast();
      isOpen.value = false;
    } catch (error: any) {
      if (error.status === 400) {
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

        failedRegisterToast();
      }
    }
  }

  return { isOpen, onSubmit };
}
