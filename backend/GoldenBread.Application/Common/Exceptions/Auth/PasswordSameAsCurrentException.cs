using GoldenBread.Application.Common.Exceptions.Domain;

namespace GoldenBread.Application.Common.Exceptions.Auth;

public class PasswordSameAsCurrentException : DuplicateEntityException
{
    public PasswordSameAsCurrentException()
        : base("NewPassword", "Новый пароль не должен совпадать со старым") { }
}
