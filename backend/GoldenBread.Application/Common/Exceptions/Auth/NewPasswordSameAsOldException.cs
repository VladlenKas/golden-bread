using GoldenBread.Application.Common.Exceptions.Domain;

namespace GoldenBread.Application.Common.Exceptions.Auth;

public class NewPasswordSameAsOldException : DuplicateEntityException
{
    public NewPasswordSameAsOldException()
        : base("NewPassword", "Новый пароль не должен совпадать со старым") { }
}
