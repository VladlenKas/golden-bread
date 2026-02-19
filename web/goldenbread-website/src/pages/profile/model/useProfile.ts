// pages/profile/model/useProfile.ts
import { ref, onMounted, computed } from 'vue';
import { useProfileStore } from '@/entities/profile';
import { useNotifications } from '@/shared/composables';
import { useApiErrors } from '@/shared/composables/useApiErrors';
import { ErrorKind } from '@/shared/api';
import type {
  UpdateRequisitesRequest,
  UpdateContactsRequest,
  ChangePasswordRequest,
  ChangeEmailRequest,
} from '@/entities/profile';

export function useProfile() {
  const profileStore = useProfileStore();
  const { successToast, errorApiToast } = useNotifications();
  const { handleFieldErrors, isHandledError } = useApiErrors();

  const isLoading = ref(false);

  const profile = computed(() => profileStore.profile);
  const requisites = computed(() => profileStore.requisites);
  const contacts = computed(() => profileStore.contacts);
  const credentials = computed(() => profileStore.credentials);

  onMounted(async () => {
    if (!profileStore.isProfileLoaded) {
      await loadProfile();
    }
  });

  async function loadProfile() {
    isLoading.value = true;
    try {
      await profileStore.loadProfile();
    } catch (error: any) {
      if (error.kind !== ErrorKind.Network) {
        errorApiToast(error.message, error.status);
      }
    } finally {
      isLoading.value = false;
    }
  }

  async function handleUpdateRequisites(
    values: UpdateRequisitesRequest,
    setErrors: (errors: Record<string, string>) => void,
  ) {
    try {
      await profileStore.updateRequisites(values);
      successToast(
        'Реквизиты обновлены. Аккаунт отправлен на повторную верификацию',
      );
    } catch (error: any) {
      const fieldMapping: Record<string, string> = {
        Name: 'name',
        Inn: 'inn',
        Ogrn: 'ogrn',
      };

      if (
        !handleFieldErrors(error, setErrors, fieldMapping) &&
        error.kind !== ErrorKind.Network
      ) {
        errorApiToast(error.message, error.status);
      }
    }
  }

  async function handleUpdateContacts(
    values: UpdateContactsRequest,
    setErrors: (errors: Record<string, string>) => void,
  ) {
    try {
      await profileStore.updateContacts(values);
      successToast('Контактные данные успешно обновлены');
    } catch (error: any) {
      const fieldMapping: Record<string, string> = {
        Phone: 'phone',
        Address: 'address',
      };

      if (
        !handleFieldErrors(error, setErrors, fieldMapping) &&
        error.kind !== ErrorKind.Network
      ) {
        errorApiToast(error.message, error.status);
      }
    }
  }

  async function handleChangePassword(
    values: ChangePasswordRequest,
    setErrors: (errors: Record<string, string>) => void,
  ): Promise<boolean> {
    try {
      await profileStore.changePassword(values);
      successToast('Пароль успешно изменен');
      return true;
    } catch (error: any) {
      const fieldMapping: Record<string, string> = {
        Password: 'oldPassword',
        NewPassword: 'newPassword',
      };

      if (
        !handleFieldErrors(error, setErrors, fieldMapping) &&
        error.kind !== ErrorKind.Network
      ) {
        errorApiToast(error.message, error.status);
      }
      return false;
    }
  }

  async function handleChangeEmail(
    values: ChangeEmailRequest,
    setErrors: (errors: Record<string, string>) => void,
  ): Promise<boolean> {
    try {
      await profileStore.changeEmail(values);
      successToast('Email изменен. Аккаунт отправлен на повторную верификацию. Проверьте новую почту');
      return true;
    } catch (error: any) {
      const fieldMapping: Record<string, string> = {
        Email: 'newEmail',
        Password: 'password',
      };

      if (
        !handleFieldErrors(error, setErrors, fieldMapping) &&
        error.kind !== ErrorKind.Network
      ) {
        errorApiToast(error.message, error.status);
      }
      return false;
    }
  }

  return {
    isLoading,
    profile,
    requisites,
    contacts,
    credentials,
    loadProfile,
    handleUpdateRequisites,
    handleUpdateContacts,
    handleChangePassword,
    handleChangeEmail,
  };
}
