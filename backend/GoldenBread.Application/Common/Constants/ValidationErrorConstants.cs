namespace GoldenBread.Application.Common.Constants;

public static class ValidationErrorConstants
{
    // Общие
    public const string Duplicate = "Уже существует";
    public const string Required = "Поле обязательно для заполнения";
    public const string NotFound = "Не найдено";

    // Пароли
    public const string InvalidPassword = "Неверный пароль";
    public const string PasswordsMismatch = "Пароли не совпадают";
    public const string NewPasswordSameAsOld = "Новый пароль должен отличаться от старого";

    // Компания/Аккаунт
    public const string InvalidCredentials = "Аккаунт не найден или не существует. Проверьте свои данные";
    public const string SessionExpired = "Сессия истекла или не существует. Выполните вход в систему заново";
}

