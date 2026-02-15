import type { VerificationStatus } from "./user";

export interface AuthResponse {
  verificationStatus: VerificationStatus;
}

export interface RegisterRequest {
  name: string;
  inn: string;
  ogrn: string;
  email: string;
  password: string;
}