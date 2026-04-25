using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GoldenBread.Desktop.Configuration.Services;
using GoldenBread.Desktop.Features.Administration.Accounts;
using GoldenBread.Desktop.Features.Administration.Companies;
using GoldenBread.Desktop.Features.Administration.SystemUsers;
using GoldenBread.Desktop.Features.Auth;
using GoldenBread.Desktop.Features.Menu;
using GoldenBread.Desktop.Features.Orders.OrdersList;
using GoldenBread.Desktop.Features.Preview;
using GoldenBread.Desktop.Features.Procurement.PurchasePositions;
using GoldenBread.Desktop.Features.Procurement.Warehouse;
using GoldenBread.Desktop.Features.Production.EmployeeTasks;
using GoldenBread.Desktop.Features.Production.ProductBatches;
using GoldenBread.Desktop.Features.Production.Recipes;
using GoldenBread.Desktop.Features.References.Employees;
using GoldenBread.Desktop.Features.References.Ingredients;
using GoldenBread.Desktop.Features.References.Products;
using GoldenBread.Desktop.Features.References.Suppliers;
using GoldenBread.Desktop.Infrastructure.Api.Clients;
using GoldenBread.Desktop.Infrastructure.Auth;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Services.Dialogs;
using GoldenBread.Desktop.UI.Services.Tosts;
using GoldenBread.Desktop.UI.Services.Views;
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

    private static ServiceProvider ConfigureServices(IServiceCollection services)
    {
        // UI
        services.AddSingleton<IPageFactory, PageFactory>();
        services.AddSingleton<IWindowService, WindowService>();
        services.AddSingleton<IToastService, ToastService>();
        services.AddSingleton<IDialogService, DialogService>();

        // Infra
        services.AddTransient<SessionHeaderHandler>();
        services.AddSingleton<IMenuConfigService, MenuConfigService>();
        services.AddSingleton<ISessionStorage, DataProtectionSessionStorage>();
        services.AddSingleton<ICurrentUserStore, CurrentUserStore>();

        // ViewModels - Windows
        services.AddTransient<PreviewWindowView>();
        services.AddTransient<PreviewWindowViewModel>();
        services.AddTransient<AuthWindowView>();
        services.AddTransient<AuthWindowViewModel>();
        services.AddTransient<MenuWindowView>();
        services.AddTransient<MenuWindowViewModel>();

        // ViewModels - Pages
        services.AddTransient<ProductsPageViewModel>();
        services.AddTransient<IngredientsPageViewModel>();
        services.AddTransient<SuppliersPageViewModel>();
        services.AddTransient<EmployeesPageViewModel>();
        services.AddTransient<PurchasePositionsPageViewModel>();
        services.AddTransient<WarehousePageViewModel>();
        services.AddTransient<RecipesPageViewModel>();
        services.AddTransient<ProductBatchesPageViewModel>();
        services.AddTransient<EmployeeTasksPageViewModel>();
        services.AddTransient<OrdersListPageViewModel>();
        services.AddTransient<SystemUsersPageViewModel>();
        services.AddTransient<CompaniesPageViewModel>();
        services.AddTransient<AccountsPageViewModel>();

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
    public static IServiceCollection AddApiClient<TInterface>(this IServiceCollection services) 
        where TInterface : class
    {
        services.AddRefitClient<TInterface>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(Paths.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(10);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddHttpMessageHandler<SessionHeaderHandler>();

        return services;
    }
}