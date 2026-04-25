using GoldenBread.Desktop.Features.Auth;
using GoldenBread.Desktop.Features.Menu;
using GoldenBread.Desktop.Infrastructure.Api.Clients;
using GoldenBread.Desktop.Infrastructure.Auth;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services.Dialogs;
using GoldenBread.Desktop.UI.Services.Windows;
using ReactiveUI.SourceGenerators;
using System.Diagnostics;

namespace GoldenBread.Desktop.Features.Preview;

public partial class PreviewWindowViewModel(
    IWindowService windowService,
    ISessionStorage sessionStorage,
    IDialogService dialogService,
    ICurrentUserStore userStore,
    IAuthApi authApi) : ViewModelBase
{
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

            var session = sessionStorage.LoadSession();
            if (!string.IsNullOrEmpty(session))
            {
                StatusText = "Получение данных...";

                // Проверяем валидность сессии
                var response = await authApi.Me();

                if (response.IsSuccessStatusCode && response.Content != null)
                {
                    // Загружаем актуальные данные пользователя
                    var data = response.Content;
                    userStore.Authenticate(data.Id, data.Role!.Value, data.VerificationStatus);

                    // Главное окно
                    StatusText = "Открытие главного меню...";

                    windowService.ShowWindow<MenuWindowView, MenuWindowViewModel>();
                    windowService.CloseWindow(this);
                    return;
                }
            }

            sessionStorage.Clear();

            // Окно авторизации
            StatusText = "Открытие окна авторизации...";

            windowService.ShowWindow<AuthWindowView, AuthWindowViewModel>();
            windowService.CloseWindow(this);
        }
        catch (Exception)
        {
            dialogService.ShowError(DialogManager, DialogMessages.ErrorException);
        }
        finally
        {
            IsLoading = false;
            StatusText = "Попробуйте перезапустить приложение";
        }
    }
}
