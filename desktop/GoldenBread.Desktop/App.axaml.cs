using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GoldenBread.Desktop.Api;
using GoldenBread.Desktop.Services.Dialogs;
using GoldenBread.Desktop.Services.Tosts;
using GoldenBread.Desktop.ViewModels;
using GoldenBread.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;
using System.Net.Http;
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

    public override void OnFrameworkInitializationCompleted()
    {
        var servises = new ServiceCollection();
        _serviceProvider = ConfigureServices(servises);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownRequested += OnShutdownRequested;

            var mainVm = _serviceProvider.GetRequiredService<LoginWindowViewModel>();
            desktop.MainWindow = new LoginWindow
            {
                DataContext = mainVm,
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static ServiceProvider ConfigureServices(IServiceCollection services)
    {
        // Add services
        services.AddSingleton<IToastService, ToastService>();
        services.AddSingleton<IDialogService, DialogService>();

        // Add view models
        services.AddTransient<LoginWindowViewModel>();

        // Add api
        services.AddRefitClient<IUserApi>(new RefitSettings())
            .ConfigureHttpClient(ConfigureClient);

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