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
        const apiErrors = error.data?.errors || {};

        setErrors({
          name: apiErrors.Name?.[0],
          email: apiErrors.Email?.[0],
          password: apiErrors.Password?.[0],
          inn: apiErrors.Inn?.[0],
          ogrn: apiErrors.Ogrn?.[0],
        });

        failedRegisterToast();
      }
    }
  }

  return { isOpen, onSubmit };
}
