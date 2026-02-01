import type {
  RegisterRequest,
  RegisterResponse,
} from '../register/registerDto';
import { api } from '../http/api';

export async function registerService(
  registerRequest: RegisterRequest,
): Promise<RegisterResponse> {
  const { data } = await api.post<RegisterResponse>(
    '/api/Register/company',
    registerRequest,
  );
  return data;
}
