import type { AxiosError } from 'axios';

export class ApiError extends Error {
  constructor(
    message: string,
    public status?: number,
    public data?: unknown,
  ) {
    super(message);
    this.name = 'ApiError';
  }
}

export function createApiError(error: AxiosError): ApiError {
  const { message } = error;
  const status = error.response?.status;
  const data = error.response?.data;

  return new ApiError(message, status, data);
}
