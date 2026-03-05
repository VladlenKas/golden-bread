using GoldenBread.Application.Common.Exceptions.Validation;

namespace GoldenBread.Application.Common.Exceptions.Domain;

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


