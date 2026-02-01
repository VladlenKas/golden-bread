import type { LoginRequest, LoginResponse } from '../login/loginDto';
import { api } from '../http/api';

export async function loginService(
  loginRequest: LoginRequest,
): Promise<LoginResponse> {
  console.log('Отправляем запрос на апи');
  const { data } = await api.post<LoginResponse>(
    '/api/Login/company',
    loginRequest,
  );
  return data;
}
