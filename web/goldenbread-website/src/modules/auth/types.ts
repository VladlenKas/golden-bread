export interface AuthResponse {
  id: number;
  verificationStatus: VerificationStatus
} 

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  name: string;
  inn: string;
  ogrn: string;
  email: string;
  password: string;
}

export interface User {
  id: number;
  name: string;
  inn: string;
  ogrn: string;
  email: string;
  password: string;
}

export enum VerificationStatus {
  Pending = 'Pending',
  Approved = 'Approved',
  Rejected = 'Rejected',
  Suspended = 'Suspended',
}