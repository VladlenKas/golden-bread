using Avalonia.Controls.Notifications;
using SukiUI.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Services.Dialogs;

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
}
