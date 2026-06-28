import { ref } from 'vue';
import { me as meApi } from '../auth/api'
import { createDeliveryInvoicePdf, createCooperationAgreementPdf } from './api';

export function useDocuments() {
  const isDownloadingInvoice = ref<number | null>(null);
  const isDownloadingAgreement = ref(false);

  const downloadDeliveryInvoice = async (orderId: number) => {
    isDownloadingInvoice.value = orderId;
    try {
      const response = await createDeliveryInvoicePdf(orderId);
      const blob = new Blob([response.data], { type: 'application/pdf' });
      const fileName = response.headers['content-disposition']?.split('filename=')[1]?.replace(/["']/g, '')
        || `Накладная_№${orderId}.pdf`;
      downloadBlob(blob, fileName);
    } finally {
      isDownloadingInvoice.value = null;
    }
  };

  const downloadCooperationAgreement = async () => {
    isDownloadingAgreement.value = true;
    try {

      const account = await meApi();
      const response = await createCooperationAgreementPdf(account.id);
      const blob = new Blob([response.data], { type: 'application/pdf' });
      const fileName = response.headers['content-disposition']?.split('filename=')[1]?.replace(/["']/g, '')
        || 'Договор_сотрудничества.pdf';
      downloadBlob(blob, fileName);
    } finally {
      isDownloadingAgreement.value = false;
    }
  };

  const downloadBlob = (blob: Blob, fileName: string) => {
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  };

  return {
    isDownloadingInvoice,
    isDownloadingAgreement,
    downloadDeliveryInvoice,
    downloadCooperationAgreement,
  };
}