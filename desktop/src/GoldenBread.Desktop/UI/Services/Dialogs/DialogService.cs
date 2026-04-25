using Avalonia.Controls.Notifications;
using SukiUI.Dialogs;

namespace GoldenBread.Desktop.UI.Services.Dialogs;

public class DialogService : IDialogService
{
    public void ShowError(ISukiDialogManager manager, string message)
    {
        manager.CreateDialog()
            .WithTitle("Ошибка")
            .WithContent(message)
            .OfType(NotificationType.Error)
            .Dismiss().ByClickingBackground()
            .TryShow();
    }

    public void ShowInfo(ISukiDialogManager manager, string message)
    {
        manager.CreateDialog()
            .WithTitle("Информация")
            .WithContent(message)
            .OfType(NotificationType.Information)
            .Dismiss().ByClickingBackground()
            .TryShow();
    }

    public void ShowSuccess(ISukiDialogManager manager, string message)
    {
        manager.CreateDialog()
            .WithTitle("Успех")
            .WithContent(message)
            .OfType(NotificationType.Success)
            .Dismiss().ByClickingBackground()
            .TryShow();
    }

    public void ShowWarning(ISukiDialogManager manager, string message)
    {
        manager.CreateDialog()
            .WithTitle("Предупреждение")
            .WithContent(message)
            .OfType(NotificationType.Warning)
            .Dismiss().ByClickingBackground()
            .TryShow();
    }

    public TaskCompletionSource<bool> ShowQustion(ISukiDialogManager manager, string message)
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
