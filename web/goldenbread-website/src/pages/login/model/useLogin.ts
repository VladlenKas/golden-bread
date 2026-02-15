import { useNotifications } from "@/shared/composables";
import { useUserStore, type LoginCredentials } from "@/entities/user";
import { VerificationStatus } from "@/shared/api";

export function useLogin() {
  const { successToast, infoToast, errorToast, errorApiToast } = useNotifications();
  const useUser = useUserStore(); 

  async function handleLogin(credentials: LoginCredentials) {
    const response = await useUser.login(credentials);

    switch (response.verificationStatus) {
      case VerificationStatus.Approved: {
        successToast("Вход успешно выполнен");
        break;
      }
      case VerificationStatus.Pending: {
        infoToast("Ваш аккаунт в настоящий момент проходит проверку. Пожалуйста, попробуйте позже");
        break;
      }
      case VerificationStatus.Rejected: {
        errorToast("Ваш аккаунт не прошел проверку. Позвоните на горячую линию для уточнения деталей");
        break;
      }
      case VerificationStatus.Suspended: {
        errorToast("Ваш аккаунт временно заморожен. Позвоните на горячую линию для уточнения деталей");
        break;
      }
    }
  } 

  return { handleLogin }
} 