using SukiUI.Dialogs;

namespace GoldenBread.Desktop.UI.Services.Dialogs;

public interface IDialogService
{
    void ShowSuccess(ISukiDialogManager manager, string message);
    void ShowError(ISukiDialogManager manager, string message);
    void ShowInfo(ISukiDialogManager manager, string message);
    void ShowWarning(ISukiDialogManager manager, string message);
    TaskCompletionSource<bool> ShowQustion(ISukiDialogManager manager, string message);
}
