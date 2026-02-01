export interface RegisterRequest {
  name: string;
  inn: string;
  ogrn: string;
  email: string;
  password: string;
}

export interface RegisterResponse {
  id: number;
  session: string;
  sessionExpiresAt: string;
  accountStatus: string;
}
