using Avalonia.Controls.Notifications;
using GoldenBread.Desktop.Infrastructure.Constants;
using SukiUI.Toasts;
using Tmds.DBus.Protocol;

namespace GoldenBread.Desktop.UI.Services;

public class ToastService(ISukiToastManager manager)
{
    public void ShowError(string? message = ConstantMessages.ExceptionToast)
    {
        manager.CreateToast()
            .WithTitle("Ошибка")
            .WithContent(message)
            .OfType(NotificationType.Error)
            .Dismiss().After(TimeSpan.FromSeconds(3))
            .Dismiss().ByClicking()
            .Queue();
    }

    public void ShowInfo(string message)
    {
        manager.CreateToast()
            .WithTitle("Информация")
            .WithContent(message)
            .OfType(NotificationType.Information)
            .Dismiss().After(TimeSpan.FromSeconds(3))
            .Dismiss().ByClicking()
            .Queue();
    }

    public void ShowSuccess(string message)
    {
        manager.CreateToast()
            .WithTitle("Успех")
            .WithContent(message)
            .OfType(NotificationType.Success)
            .Dismiss().After(TimeSpan.FromSeconds(3))
            .Dismiss().ByClicking()
            .Queue();
    }

    public void ShowWarning(string message)
    {
        manager.CreateToast()
            .WithTitle("Предупреждение")
            .WithContent(message)
            .OfType(NotificationType.Warning)
            .Dismiss().After(TimeSpan.FromSeconds(3))
            .Dismiss().ByClicking()
            .Queue();
    }

    public void ShowErrorImportant(string message)
    {
        manager.CreateToast()
            .WithTitle("Ошибка")
            .WithContent(message)
            .OfType(NotificationType.Error)
            .Dismiss().ByClicking()
            .Queue();
    }

    public TaskCompletionSource<bool> ShowWarningQuestion(string message)
    {
        var tcs = new TaskCompletionSource<bool>();

        manager.CreateToast()
            .WithTitle("Подтверждение")
            .WithContent(message)
            .OfType(NotificationType.Warning)
            .WithActionButton("Нет", _ =>
            {
                tcs.TrySetResult(false);
            }, dismissOnClick: true)
            .WithActionButton("Да", _ =>
            {
                tcs.TrySetResult(true);
            }, dismissOnClick: true)
            .Queue();

        return tcs;
    }
}
