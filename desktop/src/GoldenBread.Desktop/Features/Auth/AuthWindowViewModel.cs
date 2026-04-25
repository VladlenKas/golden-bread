using GoldenBread.Desktop.Features.Menu;
using GoldenBread.Desktop.Infrastructure.Api.Clients;
using GoldenBread.Desktop.Infrastructure.Api.Models.Auth;
using GoldenBread.Desktop.Infrastructure.Auth;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services.Dialogs;
using GoldenBread.Desktop.UI.Services.Windows;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.ComponentModel.DataAnnotations;

namespace GoldenBread.Desktop.Features.Auth;

public partial class AuthWindowViewModel(
    IWindowService windowService,
    IDialogService dialogService,
    ISessionStorage sessionStorage,
    ICurrentUserStore authState,
    IAuthApi authApi) : ViewModelBase
{
    [Reactive][Required] private string _password = null!;
    [Reactive][Required] private string _email = null!;
    [Reactive] private bool _isLoading = false;

    [ReactiveCommand]
    private async Task LoginAsync()
    {

        if (HasErrors)
        {
            dialogService.ShowError(DialogManager, ValidationMessages.EmptyField);
            return;
        }

        IsLoading = true;

        try
        {
            var request = new AuthRequest(Email, Password);
            var response = await authApi.Login(request);

            if (!response.IsSuccessStatusCode || response.Content == null)
            {
                dialogService.ShowError(DialogManager, "Пользователь не найден");
                return;
            }

            var data = response.Content;

            // Проверяем статус аккаунта
            if (data.VerificationStatus != VerificationStatus.Approved)
            {
                dialogService.ShowError(DialogManager, $"Аккаунт {data.VerificationStatus}");
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
        catch (Exception)
        {
            dialogService.ShowError(DialogManager, DialogMessages.ErrorException);
        }
        finally
        {
            IsLoading = false;
        }
    }
}
