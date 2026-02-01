export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  id: number;
  name: string;
  session: string;
  sessionExpiresAt: string;
  accountStatus: string;
}
