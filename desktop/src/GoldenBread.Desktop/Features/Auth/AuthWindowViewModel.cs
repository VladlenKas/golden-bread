using GoldenBread.Desktop.Features.Common.Models;
using GoldenBread.Desktop.Features.Menu;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.Infrastructure.Auth;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using SukiUI.Dialogs;
using System.ComponentModel.DataAnnotations;

namespace GoldenBread.Desktop.Features.Auth;

public partial class AuthWindowViewModel(
    DialogService dialogService,
    ISukiDialogManager sukiDialogManager,
    WindowService windowService,
    SessionStorage sessionStorage,
    CurrentUserStore authState,
    IAuthApi authApi) : ViewModelBase
{
    public ISukiDialogManager SukiDialogManager { get; } = sukiDialogManager;

    [Reactive][Required] private string _password = null!;
    [Reactive][Required] private string _email = null!;
    [Reactive] private bool _isLoading = false;

    [ReactiveCommand]
    private async Task LoginAsync()
    {
        if (HasErrors)
        {
            dialogService.ShowError(ConstantMessages.RequiredFieldsDialog);
            return;
        }

        IsLoading = true;

        try
        {
            var request = new AuthRequest(Email, Password);
            var response = await authApi.Login(request);

            if (!response.IsSuccessStatusCode || response.Content == null)
            {
                dialogService.ShowError(ConstantMessages.UserNotFoundDialog);
                return;
            }

            var data = response.Content;

            // Проверяем статус аккаунта
            if (data.VerificationStatus != VerificationStatus.Approved)
            {
                dialogService.ShowWarning(ConstantMessages.GetStatusMessage(data.VerificationStatus));
                return;
            }

            // Сохраняем сессию в хранилище
            if (!string.IsNullOrEmpty(data.Session))
                sessionStorage.SaveSession(data.Session);

            // Сохраняем данные пользователя локально
            authState.Authenticate(
                data.Id, 
                data.Role!.Value, 
                data.VerificationStatus);

            windowService.ShowWindow<MenuWindowView, MenuWindowViewModel>();
            windowService.CloseWindow(this);
        }
        catch
        {
            dialogService.ShowError(ConstantMessages.ExceptionDialog);
        }
        finally
        {
            IsLoading = false;
        }
    }
}
