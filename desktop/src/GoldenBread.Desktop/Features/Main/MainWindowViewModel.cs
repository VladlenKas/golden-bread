using GoldenBread.Desktop.Features.Auth;
using GoldenBread.Desktop.Features.Common.Models;
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
    WindowService windowService,
    ISukiDialogManager sukiDialogManager,
    SessionStorage sessionStorage,
    CurrentUserStore userStore,
    DialogService dialogService,
    IAuthApi authApi) : ViewModelBase
{
    [Reactive] private bool _isLoading = true;
    [Reactive] private string _statusText = "Загрузка...";

    public ISukiDialogManager SukiDialogManager { get; } = sukiDialogManager;

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

                if (data.VerificationStatus != VerificationStatus.Approved)
                {
                    dialogService.ShowWarning(ConstantMessages.GetStatusMessage(data.VerificationStatus));
                    sessionStorage.Clear();
                    return;
                }

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
        catch(Exception ex)
        {
            Debug.WriteLine($"[MainWindow]: {ex}");
        }
        finally
        {
            StatusText = "Не удалось запустить приложение";
            IsLoading = false;
        }
    }
}
