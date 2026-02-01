import { useNotifications } from '../composables/useNotifications';
import { useLoginStore } from '@/features/login/loginStore';
import { ref } from 'vue';

export function useLoginDialog() {
  const isOpen = ref(false);
  const loginStore = useLoginStore();
  const { successLoginToast, failedLoginToast } = useNotifications();

  async function onSubmit(values: any) {
    try {
      await loginStore.login(values);
      successLoginToast();
      isOpen.value = false;
    } catch (error: any) {
      if (error.status === 401) {
        failedLoginToast();
        return;
      }
    }
  }

  return { isOpen, onSubmit };
}
