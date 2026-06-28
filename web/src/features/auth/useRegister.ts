import { useFormMappingErros, useNotifications } from '@/shared/composables';
import type { RegisterRequest } from './types';
import { ErrorKind } from '@/shared/api';
import { register, createCooperationAgreementForRegistrationPdf } from './api';
import { router } from '@/app/providers/router';
import { ref } from 'vue';

const isDownloadingAgreement = ref(false);

export function useRegister() {
  const { mapErrors } = useFormMappingErros();
  const { successToast, unhandledErrorToast } = useNotifications();

  async function handleRegister(values: RegisterRequest, setErrors: any) {
    try {
      await register(values);
      successToast(
        'Регистрация прошла успешно. Мы вас оповестим, когда аккаунт будет верифицирован.',
      );
    } catch (error: any) {
      if (error.status === 422 || error.status === 409) {
        const fieldMapping = {
          Name: 'name',
          Inn: 'inn',
          Ogrn: 'ogrn',
          Email: 'email',
          Password: 'password',
        };
        setErrors(mapErrors(error, fieldMapping));
      } else if (error.kind === ErrorKind.Unknown) {
        unhandledErrorToast(error.message, error.status);
      }
    }
  }

  async function downloadPreviewAgreement(values: any) {
    isDownloadingAgreement.value = true;
    try {
      const companyInfo = {
        name: values.name,
        inn: values.inn,
        ogrn: values.ogrn,
        address: '',
        phone: '',
      };

      const response =
        await createCooperationAgreementForRegistrationPdf(companyInfo);
      const blob = new Blob([response.data], { type: 'application/pdf' });
      const fileName =
        response.headers['content-disposition']
          ?.split('filename=')[1]
          ?.replace(/["']/g, '') || `Договор_сотрудничества_${values.inn}.pdf`;

      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = fileName;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      window.URL.revokeObjectURL(url);
    } catch (error: any) {
      console.error('Failed to download agreement:', error);
      unhandledErrorToast(error.message, error.status);
    } finally {
      isDownloadingAgreement.value = false;
    }
  }

  return {
    handleRegister,
    isDownloadingAgreement,
    downloadPreviewAgreement,
  };
}
