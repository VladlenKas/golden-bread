import { ref, onMounted, computed } from 'vue';

import { useFormMappingErros, useNotifications } from '@/shared/composables';
import { ErrorKind } from '@/shared/api';

import type {
  UpdateRequisitesRequest,
  UpdateContactsRequest,
  ChangePasswordRequest,
  ChangeEmailRequest,
  ProfileResponse,
} from './types';

import {
  getProfile,
  updateRequisites,
  updateContacts,
  changePassword,
  changeEmail,
} from './api'

export function useProfile() {
  const { successToast, unhandledErrorToast } = useNotifications();
  const { mapErrors } = useFormMappingErros(); 

  const profile = ref<ProfileResponse | null>(null);
  const isLoading = ref(false);

  onMounted(loadProfile);

  // Загрузка профиля
  async function loadProfile() {
    isLoading.value = true;
    try {
      profile.value = await getProfile();
    } catch (error: any) {
      if (error.kind === ErrorKind.Unknown) 
        unhandledErrorToast(error.message, error.status);
    } finally {
      isLoading.value = false;
    }
  }

  // Обновление реквизитов
  async function handleUpdateRequisites(values: UpdateRequisitesRequest, setErrors: any) {
    try {
      await updateRequisites(values);
      successToast('Реквизиты обновлены. Аккаунт отправлен на повторную верификацию');
      window.location.reload();
    } catch (error: any) {
      if (error.status === 422 || 409) {
        const fieldMapping = { Name: 'name', Inn: 'inn', Ogrn: 'ogrn' }
        setErrors(mapErrors(error, fieldMapping));
      } else if (error.kind === ErrorKind.Unknown) {
        unhandledErrorToast(error.message, error.status);
      }
    }
  }

  // Обновление контактов
  async function handleUpdateContacts(values: UpdateContactsRequest, setErrors: any) {
    try {
      await updateContacts(values);
      successToast('Контактные данные успешно обновлены');
    } catch (error: any) {
      if (error.status === 422 || 409) {
        const fieldMapping = { Phone: 'phone', Address: 'address' }
        setErrors(mapErrors(error, fieldMapping));
      } else if (error.kind === ErrorKind.Unknown) {
        unhandledErrorToast(error.message, error.status);
      }
    }
  }

  // Смена пароля
  async function handleChangePassword(values: ChangePasswordRequest, setErrors: any) {
    try {
      await changePassword(values);
      successToast('Пароль успешно изменен');
      return true;
    } catch (error: any) {
      if (error.status === 422 || 409) {
        const fieldMapping = { Password: 'oldPassword', NewPassword: 'newPassword' }
        setErrors(mapErrors(error, fieldMapping));
      } else if (error.kind === ErrorKind.Unknown) {
        unhandledErrorToast(error.message, error.status);
      }
    }
  }

  // Смена email
  async function handleChangeEmail(values: ChangeEmailRequest, setErrors: any) {
    try {
      await changeEmail(values);
      successToast('Email изменен');
      return true;
    } catch (error: any) {
      if (error.status === 422 || 409) {  
        const fieldMapping = { Email: 'newEmail', Password: 'password' }
        setErrors(mapErrors(error, fieldMapping));
      } else if (error.kind === ErrorKind.Unknown) {
        unhandledErrorToast(error.message, error.status);
      }
    }
  }

  return {
    isLoading,
    profile,
    loadProfile,
    handleUpdateRequisites,
    handleUpdateContacts,
    handleChangePassword,
    handleChangeEmail,
  };
}