export enum VerificationStatus {
  Pending = 'Pending',
  Approved = 'Approved',
  Rejected = 'Rejected',
  Suspended = 'Suspended',
}

export interface User {
  id: number;
  name: string;
  inn: string;
  ogrn: string;
  email: string;
  password: string;
};