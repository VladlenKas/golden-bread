using Avalonia.Controls.Notifications;
using SukiUI.Controls;
using SukiUI.Toasts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.UI.Services.Tosts;

public class ToastService : IToastService
{
    public void ShowError(ISukiToastManager manager, string message)
    {
        manager.CreateToast()
            .WithTitle("Ошибка")
            .WithContent(message)
            .OfType(NotificationType.Error)
            .Dismiss().After(TimeSpan.FromSeconds(3))
            .Dismiss().ByClicking()
            .Queue();
    }

    public void ShowInfo(ISukiToastManager manager, string message)
    {
        manager.CreateToast()
            .WithTitle("Информация")
            .WithContent(message)
            .OfType(NotificationType.Information)
            .Dismiss().After(TimeSpan.FromSeconds(3))
            .Dismiss().ByClicking()
            .Queue();
    }

    public void ShowSuccess(ISukiToastManager manager, string message)
    {
        manager.CreateToast()
            .WithTitle("Успех")
            .WithContent(message)
            .OfType(NotificationType.Success)
            .Dismiss().After(TimeSpan.FromSeconds(3))
            .Dismiss().ByClicking()
            .Queue();
    }

    public void ShowWarning(ISukiToastManager manager, string message)
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
