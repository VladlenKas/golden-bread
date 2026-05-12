using Avalonia.Controls.Notifications;
using GoldenBread.Desktop.Features.Common.DetailData;
using GoldenBread.Desktop.Features.Common.ViewModels;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
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

    public TaskCompletionSource<bool> ShowWarningQuestion(string message)
    {
        var tcs = new TaskCompletionSource<bool>();

        manager.CreateDialog()
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
            .TryShow();

        return tcs;
    }

    public TaskCompletionSource<bool> ShowInfoQuestion(string message)
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

    public void ShowDetailViewModel(DetailDialogData data)
    {
        manager.CreateDialog()
            .WithTitle("Подробная информация")
            .WithViewModel(_ => new DetailDialogViewModel(data), false)
            .Dismiss().ByClickingBackground()
            .TryShow();
    }

    public TaskCompletionSource<bool> ShowDialogAsync(ViewModelBase vm, string title)
    {
        var tcs = new TaskCompletionSource<bool>();
        var builder = manager.CreateDialog()
            .WithTitle(title)
            .WithViewModel(_ => vm, false);

        if (vm is IDialogAware aware)
        {
            aware.SetDialogCompletionSource(tcs);
            aware.SetDismissAction(() => manager.TryDismissDialog(builder.Dialog));
        }

        builder.TryShow();
        return tcs;
    }
}
