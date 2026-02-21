namespace GoldenBread.Application.Exceptions;

public class AccountNotFoundException : UnauthorizedAccessException
{
    public AccountNotFoundException()
        : base("Аккаунт не найден или не существует. Проверьте свои данные") { }
}