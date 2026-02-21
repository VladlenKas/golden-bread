import { useNotifications } from "@/shared/composables";
import { VerificationStatus, type AuthResponse, type LoginRequest } from "./types";
import { login } from "./api";
import { useAuthStore } from "./stores";
import { router } from "@/app/providers/router";

export function useLogin() {
  const { successToast, infoToast } = useNotifications();
  const authStore = useAuthStore();
  
  async function handleLogin(value: LoginRequest) {
    const account: AuthResponse = await login(value);

    switch (account.verificationStatus) {
      case VerificationStatus.Approved: {
        authStore.setAuthenticated(true);
        successToast("Вход успешно выполнен");
        router.push('/profile')
        break;
      }
      case VerificationStatus.Pending: {
        infoToast("Ваш аккаунт в настоящий момент проходит проверку. Пожалуйста, попробуйте позже");
        break;
      }
      case VerificationStatus.Rejected: {
        infoToast("Ваш аккаунт не прошел проверку. Позвоните на горячую линию для уточнения деталей");
        break;
      }
      case VerificationStatus.Suspended: {
        infoToast("Ваш аккаунт временно заморожен. Позвоните на горячую линию для уточнения деталей");
        break;
      }
    }
  } 

  return { handleLogin }
} 