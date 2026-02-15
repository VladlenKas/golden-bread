import { toast } from '@/shared/ui/toast';

export function useNotifications() {
  
  const successToast = (msg: string) =>
    toast({
      title: "Успех",
      description: `${msg}`,
    });
    
    const infoToast = (msg: string) =>
    toast({
      title: "Уведомление",
      description: `${msg}`,
    });
    
    const warningToast = (msg: string) =>
    toast({
      title: "Предупреждение",
      description: `${msg}`,
    });
    
    const errorToast = (msg: string) =>
    toast({
      title: "Ошибка",
      description: `${msg}`,
    });
    
    const errorApiToast = (msg: string, st: number) =>
      toast({
        title: "Критическая ошибка",
        description: `Статус: ${st}. ${msg}`,
        variant: 'destructive',
      });

  return {
    successToast,
    infoToast,
    warningToast,
    errorToast,
    errorApiToast,
  };
}
