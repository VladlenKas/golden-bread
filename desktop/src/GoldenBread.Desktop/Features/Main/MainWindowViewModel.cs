using GoldenBread.Desktop.Features.Auth;
using GoldenBread.Desktop.Features.Menu;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.Infrastructure.Auth;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;
using ReactiveUI.SourceGenerators;
using SukiUI.Dialogs;
using System.Diagnostics;

namespace GoldenBread.Desktop.Features.Main;

public partial class MainWindowViewModel(
    DialogService dialogService,
    ISukiDialogManager sukiDialogManager,
    WindowService windowService,
    SessionStorage sessionStorage,
    CurrentUserStore userStore,
    IAuthApi authApi) : ViewModelBase
{
    public ISukiDialogManager SukiDialogManager { get; } = sukiDialogManager;

    [Reactive] private bool _isLoading = true;
    [Reactive] private string _statusText = "Загрузка...";

    [ReactiveCommand]
    public async Task InitializeAsync()
    {
        IsLoading = true;
        StatusText = "Загрузка...";

        await Task.Delay(1500);

        try
        {
            StatusText = "Проверка сессии...";

            var response = await authApi.Me();

            if (response.IsSuccessStatusCode && response.Content != null)
            {
                StatusText = "Получение данных...";

                var data = response.Content;
                userStore.Authenticate(data.Id, data.Role!.Value, data.VerificationStatus);

                StatusText = "Открытие главного меню...";

                windowService.ShowWindow<MenuWindowView, MenuWindowViewModel>();
                windowService.CloseWindow(this);
                return;
            }

            sessionStorage.Clear();

            // Окно авторизации
            StatusText = "Открытие окна авторизации...";

            windowService.ShowWindow<AuthWindowView, AuthWindowViewModel>();
            windowService.CloseWindow(this);
        }
        catch
        {
            dialogService.ShowError(ConstantMessages.ErrorException);
        }
        finally
        {
            IsLoading = false;
            StatusText = "Попробуйте перезапустить приложение";
        }
    }
}
