using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GoldenBread.Desktop.Configuration.Files;
using GoldenBread.Desktop.Configuration.Services;
using GoldenBread.Desktop.Features.Administration.Accounts;
using GoldenBread.Desktop.Features.Administration.Companies;
using GoldenBread.Desktop.Features.Administration.SystemUsers;
using GoldenBread.Desktop.Features.Auth;
using GoldenBread.Desktop.Features.Main;
using GoldenBread.Desktop.Features.Menu;
using GoldenBread.Desktop.Features.Orders.OrdersList;
using GoldenBread.Desktop.Features.Procurement.PurchasePositions;
using GoldenBread.Desktop.Features.Procurement.Warehouse;
using GoldenBread.Desktop.Features.Production.EmployeeTasks;
using GoldenBread.Desktop.Features.Production.ProductBatches;
using GoldenBread.Desktop.Features.Production.Recipes;
using GoldenBread.Desktop.Features.References.Employees.ViewModels;
using GoldenBread.Desktop.Features.References.Ingredients;
using GoldenBread.Desktop.Features.References.Products;
using GoldenBread.Desktop.Features.References.Suppliers;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.Infrastructure.Auth;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using SukiUI.Dialogs;
using SukiUI.Toasts;
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
        _serviceProvider = ConfigureViewModels(servises);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownRequested += OnShutdownRequested;

            var mainVm = _serviceProvider.GetRequiredService<MainWindowViewModel>();
            var mainWindow = _serviceProvider.GetRequiredService<MainWindowView>();
            mainWindow.DataContext = mainVm;

            var windowService = _serviceProvider.GetRequiredService<WindowService>();
            windowService.RegisterWindow(mainVm, mainWindow);

            mainWindow.Show();
            await mainVm.InitializeAsync();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static ServiceProvider ConfigureServices(IServiceCollection services)
    {
        // UI
        services.AddSingleton<PageFactory>();
        services.AddSingleton<WindowService>();

        services.AddSingleton<ISukiDialogManager, SukiDialogManager>();
        services.AddSingleton<ISukiToastManager, SukiToastManager>();
        services.AddSingleton<DialogService>();
        services.AddSingleton<ToastService>();

        // Infra
        services.AddTransient<SessionHeaderHandler>();
        services.AddSingleton<CurrentUserStore>();

        // Conf
        services.AddSingleton<MenuConfigService>();
        services.AddSingleton<SessionStorage>();

        // Refit 
        services.AddApiClient<IAuthApi>();
        services.AddApiClient<IEmployeesApi>();

        return services.BuildServiceProvider();
    }

    private static ServiceProvider ConfigureViewModels(IServiceCollection services)
    {
        // Main Window (Loading)
        services.AddTransient<MainWindowView>();
        services.AddTransient<MainWindowViewModel>();

        // Auth Window
        services.AddTransient<AuthWindowView>();
        services.AddTransient<AuthWindowViewModel>();

        // Menu Window
        services.AddTransient<MenuWindowView>();
        services.AddTransient<MenuWindowViewModel>();

        // Employee Pages
        services.AddTransient<EmployeesHostPageViewModel>();
        services.AddTransient<EmployeesListPageViewModel>();
        services.AddTransient<EmployeeEditorPageViewModel>();

        // Other Pages
        services.AddTransient<ProductsHostPageViewModel>();
        services.AddTransient<IngredientsHostPageViewModel>();
        services.AddTransient<SuppliersHostPageViewModel>();
        services.AddTransient<PurchasePositionsHostPageViewModel>();
        services.AddTransient<WarehouseHostPageViewModel>();
        services.AddTransient<RecipesHostPageViewModel>();
        services.AddTransient<ProductBatchesHostPageViewModel>();
        services.AddTransient<EmployeeTasksHostPageViewModel>();
        services.AddTransient<OrdersHostPageViewModel>();
        services.AddTransient<SystemUsersHostPageViewModel>();
        services.AddTransient<CompaniesHostPageViewModel>();
        services.AddTransient<AccountsHostPageViewModel>();

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
    public static IServiceCollection AddApiClient<TInterface>(this IServiceCollection services) 
        where TInterface : class
    {
        services.AddRefitClient<TInterface>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(AppSettings.ApiUrl);
                client.Timeout = TimeSpan.FromSeconds(10);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddHttpMessageHandler<SessionHeaderHandler>();

        return services;
    }
}