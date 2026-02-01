import { MESSAGES } from '@/utils/messages';
import { toast } from '@/components/ui/toast';

export function useNotifications() {
  // Default
  
  const errorApiToast = (msg: string, st: number) =>
    toast({
      title: MESSAGES.TOAST_API_ERROR,
      description: `Ошибка ${st}: ${msg}`,
      variant: 'destructive',
    });

  // Optional
  const successLoginToast = () =>
    toast({
      title: MESSAGES.LOGIN_SUCCESS,
      description: MESSAGES.LOGIN_SUCCESS_DESC,
    });

  const failedLoginToast = () =>
    toast({
      title: MESSAGES.LOGIN_FAILED,
      description: MESSAGES.LOGIN_FAILED_DESC,
    });

  const successRegisterToast = () =>
    toast({
      title: MESSAGES.REGISTER_SUCCESS,
      description: MESSAGES.REGISTER_SUCCESS_DESC,
    });

  const failedRegisterToast = () =>
    toast({
      title: MESSAGES.REGISTER_FAILED,
      description: MESSAGES.REGISTER_FAILED_DESC,
    });

  return {
    successLoginToast,
    failedLoginToast,
    successRegisterToast,
    failedRegisterToast,
    errorApiToast,
  };
}
