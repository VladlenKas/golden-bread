namespace GoldenBread.Application.Common.Exceptions.Validation;

public record class ValidationError(string PropertyName, string ErrorMessage);
