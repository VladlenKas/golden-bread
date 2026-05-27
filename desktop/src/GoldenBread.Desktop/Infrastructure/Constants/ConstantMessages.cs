using GoldenBread.Desktop.Features.Common;

namespace GoldenBread.Desktop.Infrastructure.Constants;

public static class ConstantMessages
{
    #region Dialog Messages

    // Error
    public const string UserNotFoundDialog = "Пользователь не найден";
    public const string RequiredFieldsDialog = "Заполните все обязательные поля";

    // Exception  
    public const string ExceptionDialog = "Не удалось выполнить запрос";

    // Warning
    public const string AccountPendingDialog = "В настоящее время аккаунт находится на проверке. Попробуйте войти позже или свяжитесь со службой поддержки";
    public const string AccountRejectedDialog = "Аккаунт заблокирован. Для получения дополнительной информации свяжитесь со службой поддержки";
    public const string AccountSuspendedDialog = "В настоящее время аккаунт временно заморожен. Попробуйте войти позже или свяжитесь со службой поддержки";

    // Confirm
    public const string LogoutConfirmDialog = "Вы уверены, что хотите выйти из учетной записи? Текущая сессия будет завершена";
    public const string SupplierDeleteConfirmDialog = "Вы уверены, что хотите удалить выбранного поставщика?";
    public const string CompanyDeleteConfirmDialog = "Вы уверены, что хотите удалить выбранную компанию?";
    public const string EmployeeDismissConfirmDialog = "Вы уверены, что хотите уволить выбранного сотрудника?";
    public const string UpdateEmailConfirmDialog = "Вы уверены, что хотите сменить адрес электронной почты?";
    public const string UpdatePasswordConfirmDialog = "Вы уверены, что хотите обновить старый пароль?";
    public const string UpdateAccountStatusConfirmDialog = "Сменить статус пользователя?";
    public const string SupplierIngredientDeleteConfirmDialog = "Вы уверены, что хотите удалить эту закупочную позицию? " +
        "Будут архивированы остатки на складе в размере {0} и затронет {1} партий";
    public const string ProductDeleteConfirmDialog = "Вы уверены, что хотите удалить эту продукцию? Текущие и прошлые заказы не затронуты.";

    #endregion

    #region Toast Messages

    // Success
    public const string CreatedToast = "Запись добавлена";
    public const string UpdatedToast = "Запись обновлена";
    public const string DeletedToast = "Запись удалена";
    public const string SavedToast = "Данные сохранены";
    public const string ChangesSavedToast = "Изменения сохранены";
    public const string EmployeeDismissedToast = "Сотрудник уволен";
    public const string UpdateAccountStatusToast = "Статус обновлен";

    // Info
    public const string EmptySelectedItem = "Выберите данные из списка";
    public const string NoChangesToast = "Вы не внесли изменений";

    // Error
    public const string ErrorLoadDataToast = "Не удалось загрузить дополнительные данные";

    // Exception
    public const string ExceptionToast = "Не удалось выполнить действие";

    // Warning
    public const string SelfActionNotAllowed = "Нельзя выполнить это действие над собственной учётной записью";
    public const string SupplierCannotBeDeleted = "Выбранный поставщик не может быть удален, так как имеет активные закупочные позиции";
    public const string CompanyCannotBeDeleted = "Выбранная компания не может бытть удалена, так как имеет активные или необработанные заказы";
    public const string EmployeeCannotBeDeleted = "Выбранный сотрудник не может быть уволен, так как имеет активные или необработанные задачи";
    public const string IngredientCannotBeDeleted = "Выбранный ингредиент не может быть удален, так как имеет {0} связных рецептов и {1} закупочных позиций";
    public const string CategoryCannotBeDeleted = "Категория не может быть удалена, так как содержит {0} продукций";

    #endregion

    #region UI Titles

    public const string CreateTitlePage = "Добавление";
    public const string EditorTitlePage = "Редактирование";
    public const string HostTitlePage = "Список данных";

    #endregion

    #region Validation Messages

    // Required
    public const string RequiredValidation = "Обязательное поле не может быть пустым";
    public const string SupplierRequiredValidation = "Необходимо выбрать поставщика";
    public const string IngredientRequiredValidation = "Необходимо выбрать ингредиент";
    public const string CategoryRequiredValidation = "Необходимо выбрать категорию товара";
    public const string RequiredRoleValidation = "У пользователя должна быть выбрана должность";
    public const string RequiredUnitValidation = "Для ингредиента должна быть выбрана единица измерения";

    // Length
    public const string NameLengthValidation = "Длина поля должна быть от 2 до 35 символов";
    public const string PasswordLengthValidation = "Пароль должен содержать от 6 до 100 символов";
    public const string SupplierNameLengthValidation = "Название должно содержать от 2 до 100 символов";
    public const string CompanyNameLengthValidation = "Название должно содержать от 2 до 100 символов";
    public const string AddressLengthValidation = "Адрес не должен превышать 200 символов";

    // Format
    public const string NameFormatValidation = "Поле должно содержать только кириллицу и разделительные знаки без повторов";
    public const string NotRequiredNameFormatValidation = "Поле должно содержать только кириллицу и разделительные знаки без повторов длиной от 2 до 35 символов";
    public const string SupplierNameFormatValidation = "Название может содержать только буквы, пробелы, дефисы и ковычки";
    public const string EmailFormatValidation = "Некорректный формат электронной почты";
    public const string PhoneFormatValidation = "Телефон может содержать только цифры";
    public const string DetailsValidation = "Поле должно содержать только кириллицу и разделительные знаки";
    public const string InnFormatValidation = "ИНН должен содержать 10 цифр и не может состоять только из нулей";
    public const string OgrnFormatValidation = "ОГРН должен содержать 13 цифр, начинаться с 1 или 5, содержать год регистрации не ранее 2002 и корректный код субъекта РФ";
    public const string ProductNameLengthValidation = "Название должно содержать от 2 до 100 символов";
    public const string DescriptionLengthValidation = "Описание не должно превышать 500 символов";
    public const string CostPriceRangeValidation = "Себестоимость должна быть от 0.01 до 999999.99";
    public const string ProductionTimeRangeValidation = "Время производства должно быть от 1 до 1440 минут";
    public const string ProductShelfLifeRangeValidation = "Срок хранения должен быть от 1 до 60 дней";
    public const string StorageTempRangeValidation = "Температура хранения должна быть от -50 до 100 °C";

    // Other
    public const string PasswordsMismatchValidation = "Пароли не совпадают";
    public const string PriceRangeValidation = "Цена должна быть от 0.01 до 999999.99";
    public const string WeightRangeValidation = "Вес/объем должен быть от 0.01 до 999999.99";
    public const string ShelfLifeRangeValidation = "Срок хранения должен быть от 1 до 1200 дней";
    public const string TemperatureRangeValidation = "Минимальная температура должна быть меньше максимальной";
    public const string IngredientExsist = "Этот ингредиент уже добавлен";
    public const string IngredientsNull = "Добавьте хотя бы один ингредиент";
    public const string StorageTempValidation = "Минимальная температура хранения не может быть больше максимальной";

    
    #endregion

    #region Helper Methods

    public static string GetStatusMessage(VerificationStatus status) =>
        status switch
        {
            VerificationStatus.Pending => AccountPendingDialog,
            VerificationStatus.Rejected => AccountRejectedDialog,
            VerificationStatus.Suspended => AccountSuspendedDialog,
            _ => "Неизвестный статус"
        };

    public static string GetUpdateStatusMessage(VerificationStatus status)
    {
        string statusText = status switch
        {
            VerificationStatus.Pending => "На рассмотрении",
            VerificationStatus.Approved => "Подтверждён",
            VerificationStatus.Rejected => "Отклонён",
            VerificationStatus.Suspended => "Приостановлен",
            _ => "Неизвестный статус"
        };

        return $"Статус обновлен на \"{statusText}\"";
    }

    #endregion
}