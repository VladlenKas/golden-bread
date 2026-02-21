export function useFormMappingErros() {
  function mapErrors(error: any, fieldMapping: Record<string, string>): Record<string, string> {
    const apiErrors = error.data.errors || [];
    const groupedErrors = apiErrors.reduce((acc: any, err: any) => {
      const fieldName = fieldMapping[err.propertyName] || err.propertyName.toLowerCase();
      acc[fieldName] = err.errorMessage;
      return acc;
    }, {});

    return groupedErrors;
  }

  return { mapErrors }
}
