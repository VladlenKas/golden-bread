using Avalonia.Controls.Notifications;
using GoldenBread.Desktop.Infrastructure.Constants;
using SukiUI.Dialogs;

namespace GoldenBread.Desktop.UI.Services;

public class DialogService(ISukiDialogManager manager)
{
    public void ShowError(string message = ConstantMessages.ExceptionDialog)
    {
        manager.CreateDialog()
            .WithTitle("Ошибка")
            .WithContent(message)
            .OfType(NotificationType.Error)
            .Dismiss().ByClickingBackground()
            .TryShow();
    }

    public void ShowInfo(string message)
    {
        manager.CreateDialog()
            .WithTitle("Информация")
            .WithContent(message)
            .OfType(NotificationType.Information)
            .Dismiss().ByClickingBackground()
            .TryShow();
    }

    public void ShowSuccess(string message)
    {
        manager.CreateDialog()
            .WithTitle("Успех")
            .WithContent(message)
            .OfType(NotificationType.Success)
            .Dismiss().ByClickingBackground()
            .TryShow();
    }

    public void ShowWarning(string message)
    {
        manager.CreateDialog()
            .WithTitle("Предупреждение")
            .WithContent(message)
            .OfType(NotificationType.Warning)
            .Dismiss().ByClickingBackground()
            .TryShow();
    }

    public TaskCompletionSource<bool> ShowQustion(string message)
    {
        var tcs = new TaskCompletionSource<bool>();

        manager.CreateDialog()
            .WithTitle("Подтверждение")
            .WithContent(message)
            .OfType(NotificationType.Information)
            .WithActionButton("Нет", _ =>
            {
                tcs.TrySetResult(false);
            }, dismissOnClick: true)
            .WithActionButton("Да", _ =>
            {
                tcs.TrySetResult(true);
            }, dismissOnClick: true)
            .TryShow();

        return tcs;
    }
}
