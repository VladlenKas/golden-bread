namespace GoldenBread.Application.Exceptions;

public record class ValidationError(string PropertyName, string ErrorMessage);
