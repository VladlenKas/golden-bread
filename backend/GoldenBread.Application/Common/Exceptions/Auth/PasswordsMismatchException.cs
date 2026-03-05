using GoldenBread.Application.Common.Exceptions.Domain;

namespace GoldenBread.Application.Common.Exceptions.Auth;

public class PasswordsMismatchException : BusinessValidationException
{
    public PasswordsMismatchException()
        : base("Password", "Неверный пароль") { }
}
