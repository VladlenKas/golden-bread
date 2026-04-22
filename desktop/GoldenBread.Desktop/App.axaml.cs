using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GoldenBread.Desktop.Features.Auth;
using GoldenBread.Desktop.Features.Menu;
using GoldenBread.Desktop.Infrastructure.Api.Clients;
using GoldenBread.Desktop.Infrastructure.Auth;
using GoldenBread.Desktop.UI.Dialogs;
using GoldenBread.Desktop.UI.Tosts;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GoldenBread.Desktop;

public partial class App : Application, IDisposable
{
    private IServiceProvider? _serviceProvider;
    private bool _disposed = false;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var servises = new ServiceCollection();
        _serviceProvider = ConfigureServices(servises);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownRequested += OnShutdownRequested;

            var window = ResolveStartupWindowAsync().GetAwaiter().GetResult();
            desktop.MainWindow = window;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private async Task<Window> ResolveStartupWindowAsync()
    {
        var sessionStorage = _serviceProvider!.GetRequiredService<ISessionStorage>();
        var authApi = _serviceProvider!.GetRequiredService<IAuthApi>();
        var authState = _serviceProvider!.GetRequiredService<IAuthState>();

        // Получаем сессию пользователя из хранилища
        var session = sessionStorage.LoadSession();

        if (!string.IsNullOrEmpty(session))
        {
            // Проверяем валидность сессии
            var response = await authApi.Me();

            if (response.IsSuccessStatusCode && response.Content != null)
            {
                // Загружаем актуальные данные пользователя
                var data = response.Content;
                authState.Authenticate(data.Id, data.Role!.Value, data.VerificationStatus);

                // Главное окно
                var mainVm = _serviceProvider!.GetRequiredService<MenuWindowViewModel>();
                return new MenuWindow { DataContext = mainVm };
            }

            sessionStorage.Clear();
        }

        // Окно авторизации
        var authVm = _serviceProvider!.GetRequiredService<AuthWindowViewModel>();
        return new AuthWindow { DataContext = authVm };
    }

    private static ServiceProvider ConfigureServices(IServiceCollection services)
    {
        // UI
        services.AddSingleton<IToastService, ToastService>();
        services.AddSingleton<IDialogService, DialogService>();

        // Infra
        services.AddSingleton<ISessionStorage, DataProtectionSessionStorage>();
        services.AddSingleton<SessionHeaderHandler>();
        services.AddSingleton<IAuthState, AuthState>();

        // View Models
        services.AddTransient<AuthWindowViewModel>();

        // Refit 
        services.AddRefitClient<IAuthApi>(new RefitSettings())
            .ConfigureHttpClient(ConfigureClient)
            .AddHttpMessageHandler<SessionHeaderHandler>();

        return services.BuildServiceProvider();
    }

    private static void ConfigureClient(HttpClient client)
    {
        client.Timeout = TimeSpan.FromSeconds(10);
        client.BaseAddress = new Uri("https://localhost:7107/api");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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