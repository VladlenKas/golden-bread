using GoldenBread.Desktop.Features.Common.Models;

namespace GoldenBread.Desktop.Infrastructure.Constants;

public static class ConstantMessages
{
    #region For Dialogs

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
    public const string LogoutConfirmDialog = "Вы действительно хотите выйти?";
    public const string EmployeeDismissConfirmDialog = "Вы уверены, что хотите уволить сотрудника?";

    #endregion

    #region For Toasts

    // Success
    public const string CreatedToast = "Запись добавлена";
    public const string UpdatedToast = "Запись обновлена";
    public const string DeletedToast = "Запись удалена";
    public const string SavedToast = "Данные сохранены";
    public const string ChangesSavedToast = "Изменения сохранены";
    public const string EmployeeResumedToast = "Работа сотрудника возобновлена";
    public const string EmployeeDismissedToast = "Сотрудник уволен";
    public const string UserDeletedToast = "Пользователь уволен";
    public const string UpdateAccountStatusToast = "Статус обновлен";

    public const string EmptySelectedItem = "Выберите данные из списка";

    // Exception
    public const string ExceptionToast = "Не удалось выполнить действие";

    // Info
    public const string NoChangesToast = "Вы не внесли изменений";

    // Warning
    public const string EmployeePausedToast = "Работа сотрудника приостановлена";

    #endregion

    #region Common

    // For UI 
    public const string CreateTitlePage = "Добавление";
    public const string EditorTitlePage = "Редактирование";
    public const string HostTitlePage = "Список данных";

    public const string SuppliersTitlePage = "Список поставщиков";
    public const string SupplierDeleteConfirmDialog = "Вы уверены, что хотите удалить этого поставщика?";
    public const string SupplierDeletedToast = "Поставщик удалён";
    public const string UserDeleteConfirmDialog = "Вы уверены, что хотите уволить этого пользователя?";
    public const string UpdateAccountStatusConfirmDialog = "Сменить статус пользователя?";

    // For Attributes, Validation
    public const string NameFormatValidation = "Поле должно содержать только кириллицу и разделительные знаки без повторов";
    public const string NotRequiredNameFormatValidation = "Поле должно содержать только кириллицу и разделительные знаки без повторов длиной от 2 до 35 символов";
    public const string NameLengthValidation = "Длина поля должна быть от 2 до 35 символов";
    public const string RequiredValidation = "Обязательное поле не может быть пустым";
    public const string PasswordLengthValidation = "Пароль должен содержать от 6 до 100 символов";
    public const string SupplierNameLengthValidation = "Название должно содержать от 2 до 100 символов";
    public const string SupplierNameFormatValidation = "Название может содержать только буквы, пробелы, дефисы и ковычки";
    public const string EmailFormatValidation = "Некорректный формат электронной почты";
    public const string PhoneFormatValidation = "Телефон может содержать только цифры";
    public const string AddressLengthValidation = "Адрес не должен превышать 200 символов";

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

    #endregion
}
