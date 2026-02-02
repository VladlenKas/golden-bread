import type { LoginRequest, LoginResponse } from '../login/loginDto';
import { api } from '../http/api';

export async function loginService(
  loginRequest: LoginRequest,
): Promise<LoginResponse> {
  console.log('Отправляем запрос на апишечку');
  const { data } = await api.post<LoginResponse>(
    '/api/Auth/login/company',
    loginRequest,
  );
  console.log('Возвращаем ответ');
  return data;
}
