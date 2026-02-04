import type { LoginRequest, LoginResponse } from '../login/loginDto';
import { api } from '../http/api';

export async function loginService(
  loginRequest: LoginRequest,
): Promise<LoginResponse> {
  const { data } = await api.post<LoginResponse>(
    '/api/Auth/login/company',
    loginRequest,
  );
  return data;
}
