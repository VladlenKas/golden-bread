namespace GoldenBread.Application.Exceptions;

public abstract class BusinessValidationException : Exception
{
    public string PropertyName { get; }
    public List<ValidationError> Error { get; } = [];

    protected BusinessValidationException(string propertyName, string message)
        : base(message)
    {
        PropertyName = propertyName;
        Error.Add(new ValidationError(propertyName, message));
    }
}

public class PasswordsMismatchException : BusinessValidationException
{
    public PasswordsMismatchException()
        : base("Password", "Неверный пароль") { }
}
