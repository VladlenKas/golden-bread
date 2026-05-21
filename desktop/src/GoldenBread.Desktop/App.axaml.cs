using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GoldenBread.Desktop.Configuration.Files;
using GoldenBread.Desktop.Configuration.Services;
using GoldenBread.Desktop.Features.Administration.Companies.ViewModels;
using GoldenBread.Desktop.Features.Administration.Users.ViewModels;
using GoldenBread.Desktop.Features.Auth;
using GoldenBread.Desktop.Features.Common.ViewModels;
using GoldenBread.Desktop.Features.Main;
using GoldenBread.Desktop.Features.Menu;
using GoldenBread.Desktop.Features.Procurement.PurchasePositions.ViewModels;
using GoldenBread.Desktop.Features.Procurement.Warehouse;
using GoldenBread.Desktop.Features.Production.OrdersList.ViewModels;
using GoldenBread.Desktop.Features.References.Employees.ViewModels;
using GoldenBread.Desktop.Features.References.Products.ViewModels;
using GoldenBread.Desktop.Features.References.Suppliers.ViewModels;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.Infrastructure.Auth;
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

        // Refit + Api
        services.AddHttpClient<GoldenBreadApiClient>(client =>
        {
            client.BaseAddress = new Uri(AppSettings.ApiUrl);
            client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
        }).AddHttpMessageHandler<SessionHeaderHandler>();

        services.AddApiClient<IAuthApi>();
        services.AddApiClient<IAccountApi>();
        services.AddApiClient<IUsersApi>();
        services.AddApiClient<IEmployeesApi>();
        services.AddApiClient<ISuppliersApi>();
        services.AddApiClient<ICompaniesApi>();
        services.AddApiClient<IIngredientsApi>();
        services.AddApiClient<ISupplierIngredientsApi>();
        services.AddApiClient<ISupplierIngredientsApi>();
        services.AddApiClient<IProductsApi>();
        services.AddApiClient<IProductCategoriesApi>();
        services.AddApiClient<IImagesApi>();
        services.AddApiClient<IOrdersApi>();
        services.AddApiClient<IDocumentsApi>();

        return services.BuildServiceProvider();
    }

    private static ServiceProvider ConfigureViewModels(IServiceCollection services)
    {
        // Main Window (Loading)
        services.AddSingleton<MainWindowView>();
        services.AddTransient<MainWindowViewModel>();

        // Auth Window
        services.AddSingleton<AuthWindowView>();
        services.AddTransient<AuthWindowViewModel>();

        // Menu Window
        services.AddSingleton<MenuWindowView>();
        services.AddTransient<MenuWindowViewModel>();

        // Employee Pages
        services.AddSingleton<EmployeesHostPageViewModel>();
        services.AddSingleton<EmployeesListPageViewModel>();
        services.AddTransient<EmployeeEditorPageViewModel>();

        // Supplier Pages
        services.AddSingleton<SuppliersHostPageViewModel>();
        services.AddSingleton<SuppliersListPageViewModel>();
        services.AddTransient<SupplierEditorPageViewModel>();

        // Users Pages
        services.AddSingleton<UsersHostPageViewModel>();
        services.AddSingleton<UsersListPageViewModel>();
        services.AddTransient<UserEditorPageViewModel>();

        // Companies Pages
        services.AddSingleton<CompaniesHostPageViewModel>();
        services.AddSingleton<CompaniesListPageViewModel>();
        services.AddTransient<CompanyEditorPageViewModel>();

        // Product Pages
        services.AddSingleton<ProductsHostPageViewModel>();
        services.AddSingleton<ProductsListPageViewModel>();
        services.AddTransient<ProductEditorPageViewModel>();
        services.AddTransient<ProductImageEditorPageViewModel>();
        services.AddTransient<ProductRecipeEditorPageViewModel>();
        services.AddTransient<ProductBatchEditorPageViewModel>();

        // Warehouse Pages
        services.AddTransient<WarehouseHostPageViewModel>();

        // Purchase Positions Pages
        services.AddSingleton<PurchasePositionsHostPageViewModel>();
        services.AddSingleton<PurchasePositionsListPageViewModel>();
        services.AddTransient<PurchasePositionEditorPageViewModel>();

        // Orders Pages
        services.AddSingleton<OrdersHostPageViewModel>();
        services.AddSingleton<OrdersListPageViewModel>();
        services.AddTransient<OrderEditorPageViewModel>();

        // Common Pages
        services.AddTransient<ChangePasswordPageViewModel>();
        services.AddTransient<ChangeEmailPageViewModel>();

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
                client.Timeout = TimeSpan.FromSeconds(20);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddHttpMessageHandler<SessionHeaderHandler>();

        return services;
    }
}