using Avalonia.Controls.Notifications;
using SukiUI.Toasts;

namespace GoldenBread.Desktop.UI.Services;

public class ToastService(ISukiToastManager manager)
{
    public void ShowError(string message)
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
            .Dismiss().After(TimeSpan.FromSeconds(13))
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
}
