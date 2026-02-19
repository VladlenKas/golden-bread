namespace GoldenBread.Application.Exceptions;

public abstract class DuplicateValueException : Exception
{
    public string PropertyName { get; }
    public List<ValidationError> Error { get; } = [];

    protected DuplicateValueException(string propertyName, string message)
        : base(message)
    {
        PropertyName = propertyName;
        Error.Add(new ValidationError(propertyName, message));
    }
}

public class EmailDuplicateException : DuplicateValueException
{
    public EmailDuplicateException()
        : base("Email", "Email уже занят") { }
}

public class InnDuplicateException : DuplicateValueException
{
    public InnDuplicateException()
        : base("Inn", "ИНН уже занят") { }
}

public class OgrnDuplicateException : DuplicateValueException
{
    public OgrnDuplicateException()
        : base("Ogrn", "ОГРН уже занят") { }
}

public class NameDuplicateException : DuplicateValueException
{
    public NameDuplicateException()
        : base("Name", "Название уже занято") { }
}

public class PhoneDuplicateException : DuplicateValueException
{
    public PhoneDuplicateException()
        : base("Phone", "Номер телефона уже занят") { }
}

public class AddressDuplicateException : DuplicateValueException
{
    public AddressDuplicateException()
        : base("Address", "Адрес уже занят") { }
}
