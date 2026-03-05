namespace GoldenBread.Application.Common.Exceptions.Domain;

public abstract class DuplicateEntityException : BusinessValidationException
{
    protected DuplicateEntityException(string entityProperty, string message)
        : base(entityProperty, message) { }
}

public class EmailDuplicateException : DuplicateEntityException
{
    public EmailDuplicateException()
        : base("Email", "Email уже занят") { }
}

public class InnDuplicateException : DuplicateEntityException
{
    public InnDuplicateException()
        : base("Inn", "ИНН уже занят") { }
}

public class OgrnDuplicateException : DuplicateEntityException
{
    public OgrnDuplicateException()
        : base("Ogrn", "ОГРН уже занят") { }
}

public class NameDuplicateException : DuplicateEntityException
{
    public NameDuplicateException()
        : base("Name", "Название уже занято") { }
}

public class PhoneDuplicateException : DuplicateEntityException
{
    public PhoneDuplicateException()
        : base("Phone", "Номер телефона уже занят") { }
}

public class AddressDuplicateException : DuplicateEntityException
{
    public AddressDuplicateException()
        : base("Address", "Адрес уже занят") { }
}


