namespace GoldenBread.Application.Common.Exceptions.Auth;

public class InvalidCredentialsException : UnauthorizedAccessException
{
    public InvalidCredentialsException()
        : base("Аккаунт не найден или не существует. Проверьте свои данные") { }
}