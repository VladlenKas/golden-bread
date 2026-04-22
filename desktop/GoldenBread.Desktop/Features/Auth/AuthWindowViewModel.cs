using Avalonia.Controls;
using GoldenBread.Desktop.Infrastructure.Api.Clients;
using GoldenBread.Desktop.Infrastructure.Api.Models.Auth;
using GoldenBread.Desktop.Infrastructure.Auth;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Dialogs;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Refit;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Features.Auth;

public partial class AuthWindowViewModel(
    IDialogService dialogService,
    ISessionStorage sessionStorage,
    IAuthState authState,
    IAuthApi authApi) : ViewModelBase
{
    [Reactive][Required] private string _password = null!;
    [Reactive][Required] private string _email = null!;
    [Reactive] private bool _isLoading = false;

    [ReactiveCommand]
    private async Task LoginAsync(Window window)
    {

        if (HasErrors)
        {
            dialogService.ShowError(DialogManager, "Заполните все поля");
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
            authState.Authenticate(data.Id, data.Role!.Value, data.VerificationStatus);

            dialogService.ShowSuccess(DialogManager, "Вход выполнен успешно");
        }
        catch (ApiException ex)
        {
            dialogService.ShowError(DialogManager, $"Ошибка сервера: {ex.StatusCode}");
        }
        finally
        {
            IsLoading = false;
        }
    }   
}
