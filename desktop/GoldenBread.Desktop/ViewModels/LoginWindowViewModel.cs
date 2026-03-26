using Avalonia.Controls;
using GoldenBread.Desktop.Api;
using GoldenBread.Desktop.Services.Dialogs;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Refit;
using SukiUI.Dialogs;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.ViewModels;

public partial class LoginWindowViewModel(
    IDialogService dialogService,
    IUserApi userApi) : ViewModelBase
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
            var request = new LoginRequest(Email, Password);
            var response = await userApi.Login(request);

            if (response.IsSuccessStatusCode)
            {
                var data = response.Content;
                dialogService.ShowSuccess(DialogManager, data!.Status.ToString());
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                dialogService.ShowError(DialogManager, "Пользователь не найден");

            }
            else if (response.StatusCode == HttpStatusCode.RequestTimeout)
            {
                dialogService.ShowError(DialogManager, "Истекло время ожидания от сервера");
            }
        }
        catch (ApiException ex)
        {
            dialogService.ShowError(DialogManager, $"API Error: {ex.StatusCode}");
        }
        finally
        {
            IsLoading = false;
        }
    }   
}
