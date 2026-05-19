namespace GoldenBread.Desktop.UI.Common;

public interface IDialogAware
{
    void SetDialogCompletionSource(TaskCompletionSource<bool> tcs);
    void SetDismissAction(Action dismiss);
}