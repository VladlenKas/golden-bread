namespace GoldenBread.Application.Exceptions;

public class SessionExpiredException : UnauthorizedAccessException
{
    public SessionExpiredException()
        : base("Сессия истекла или не существует. Выполните вход в систему заново") { }
}


