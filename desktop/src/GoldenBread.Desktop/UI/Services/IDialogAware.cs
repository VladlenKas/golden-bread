namespace GoldenBread.Desktop.UI.Services;

public interface IDialogAware
{
    void SetDialogCompletionSource(TaskCompletionSource<bool> tcs);
    void SetDismissAction(Action dismiss);
}