import { ErrorKind } from '@/shared/api';

interface ApiError {
  propertyName: string;
  errorMessage: string;
}

export function useApiErrors() {
  function handleFieldErrors(
    error: any, 
    setErrors: (errors: Record<string, string>) => void, 
    fieldMapping: Record<string, string>
  ): boolean {
    if (error.status === 422 || error.status === 409) {
      const apiErrors: ApiError[] = error.data?.errors || [];
      
      const groupedErrors = apiErrors.reduce((acc: Record<string, string>, err: ApiError) => {
        const fieldName = fieldMapping[err.propertyName] || err.propertyName.toLowerCase();
        acc[fieldName] = err.errorMessage;
        return acc;
      }, {});

      setErrors(groupedErrors);
      return true;
    }
    return false;
  }

  function isHandledError(error: any): boolean {
    return error.status === 422 || error.status === 409 || error.kind === ErrorKind.Network;
  }

  return {
    handleFieldErrors,
    isHandledError,
  };
}