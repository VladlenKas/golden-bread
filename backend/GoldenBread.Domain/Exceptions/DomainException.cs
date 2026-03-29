namespace GoldenBread.Domain.Exceptions;

public record class ValidationError(string PropertyName, string ErrorMessage);

public class DomainException : Exception
{
    public string PropertyName { get; }
    public List<ValidationError> Error { get; } = [];

    public DomainException(string propertyName, string message)
        : base(message)
    {
        PropertyName = propertyName;
        Error.Add(new ValidationError(propertyName, message));
    }


}