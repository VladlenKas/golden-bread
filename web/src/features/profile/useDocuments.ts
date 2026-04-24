import { ref } from "vue";
import { createDeliveryInvoiceXlsx } from "./api";

export function useDocuments() {
  const isDownloadingInvoice = ref<number | null>(null);

  const downloadDeliveryInvoice = async (orderId: number): Promise<void> => {
    isDownloadingInvoice.value = orderId;
    
    try {
      // Получаем response, а не только data
      const response = await createDeliveryInvoiceXlsx(orderId);
      
      // Создаем Blob из response.data (который уже Blob из-за responseType: 'blob')
      const blob = new Blob([response.data], {
        type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
      });
      
      // Имя файла из заголовка или дефолтное
      const contentDisposition = response.headers['content-disposition'];
      const fileName = contentDisposition
        ? contentDisposition.split('filename=')[1]?.replace(/["']/g, '')
        : `Накладная_№${orderId}.xlsx`;
      
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = fileName;
      
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      window.URL.revokeObjectURL(url);
      
    } catch (err) {
      console.error('Download failed:', err);
      throw err;
    } finally {
      isDownloadingInvoice.value = null;
    }
  };

  return { 
    isDownloadingInvoice,
    downloadDeliveryInvoice,
  };
}