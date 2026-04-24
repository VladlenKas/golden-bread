using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GoldenBread.Desktop.Features.Auth;
using GoldenBread.Desktop.Features.Menu;
using GoldenBread.Desktop.Features.Preview;
using GoldenBread.Desktop.Infrastructure.Api.Clients;
using GoldenBread.Desktop.Infrastructure.Auth;
using GoldenBread.Desktop.UI.Services.Dialogs;
using GoldenBread.Desktop.UI.Services.Tosts;
using GoldenBread.Desktop.UI.Services.Windows;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System.Net.Http.Headers;

namespace GoldenBread.Desktop;

public partial class App : Application, IDisposable
{
    private IServiceProvider? _serviceProvider;
    private bool _disposed = false;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        var servises = new ServiceCollection();
        _serviceProvider = ConfigureServices(servises);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownRequested += OnShutdownRequested;

            var mainVm = _serviceProvider.GetRequiredService<PreviewWindowViewModel>();
            var mainWindow = _serviceProvider.GetRequiredService<PreviewWindowView>();
            mainWindow.DataContext = mainVm;

            var windowService = _serviceProvider.GetRequiredService<IWindowService>();
            windowService.RegisterWindow(mainVm, mainWindow);

            mainWindow.Show();
            await mainVm.InitializeAsync();
        }

        base.OnFrameworkInitializationCompleted();
    }

    /*private async Task<Window> ResolveStartupWindowAsync()
    {
        var sessionStorage = _serviceProvider!.GetRequiredService<ISessionStorage>();
        var authApi = _serviceProvider!.GetRequiredService<IAuthApi>();
        var authState = _serviceProvider!.GetRequiredService<IAuthState>();

        // Получаем сессию пользователя из хранилища
        var session = sessionStorage.LoadSession();

        if (!string.IsNullOrEmpty(session))
        {
            try
            {
                // Проверяем валидность сессии
                var response = await authApi.Me();

                if (response.IsSuccessStatusCode && response.Content != null)
                {
                    // Загружаем актуальные данные пользователя
                    var data = response.Content;
                    authState.Authenticate(data.Id, data.Role!.Value, data.VerificationStatus);

                    // Главное окно
                    var menuVm = _serviceProvider!.GetRequiredService<AuthWindowViewModel>();
                    return new AuthWindowView { DataContext = menuVm };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ResolveStartup] Error: {ex}");
                throw;
            }
        }

        Debug.WriteLine($"[ResolveStartup] Clearing session due to error");
        sessionStorage.Clear();

        // Окно авторизации
        var authVm = _serviceProvider!.GetRequiredService<AuthWindowViewModel>();
        return new AuthWindowView { DataContext = authVm };
    }*/

    private static ServiceProvider ConfigureServices(IServiceCollection services)
    {
        // UI
        services.AddSingleton<IWindowService, WindowService>();
        services.AddSingleton<IToastService, ToastService>();
        services.AddSingleton<IDialogService, DialogService>();

        // Infra
        services.AddSingleton<ISessionStorage, DataProtectionSessionStorage>();
        services.AddSingleton<SessionHeaderHandler>();
        services.AddSingleton<ICurrentUserStore, CurrentUserStore>();

        // View Models
        services.AddTransient<PreviewWindowView>();
        services.AddTransient<PreviewWindowViewModel>();

        services.AddTransient<AuthWindowView>();
        services.AddTransient<AuthWindowViewModel>();

        services.AddTransient<MenuWindowView>();
        services.AddTransient<MenuWindowViewModel>();

        // Refit 
        services.AddApiClient<IAuthApi>();

        return services.BuildServiceProvider();
    }

    private void OnShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
    {
        Dispose();
    }

    public void Dispose()
    {
        if (_disposed) return;

        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}

public static class RefitServiceExtensions
{
    private const string BaseUrl = "https://localhost:7107";

    public static IServiceCollection AddApiClient<TInterface>(
        this IServiceCollection services) where TInterface : class
    {
        services.AddRefitClient<TInterface>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(10);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddHttpMessageHandler<SessionHeaderHandler>();

        return services;
    }
}