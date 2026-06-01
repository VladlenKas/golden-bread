using Avalonia.Controls;
using Avalonia.Styling;
using GoldenBread.Desktop.Features.Menu;
using GoldenBread.Desktop.UI.Common;
using Microsoft.Extensions.DependencyInjection;
using SukiUI;
using SukiUI.Controls;
using SukiUI.Enums;
using System.Diagnostics;

namespace GoldenBread.Desktop.UI.Services;

public class WindowService(IServiceProvider serviceProvider)
{
    private readonly Dictionary<ViewModelBase, SukiWindow> _openedWindows = new();

    public MenuWindowView? GetMenuWindow() => serviceProvider.GetRequiredService<MenuWindowView>();

    public void RegisterWindow(ViewModelBase viewModel, SukiWindow window)
    {
        WindowThemeNormalize(window);

        _openedWindows[viewModel] = window;
        window.Closed += (_, _) => _openedWindows.Remove(viewModel);
    }

    public void ShowWindow<TView, TViewModel>(TViewModel? viewModel = null)
        where TView : SukiWindow
        where TViewModel : ViewModelBase
    {
        var vm = viewModel ?? serviceProvider.GetRequiredService<TViewModel>();

        var window = serviceProvider.GetRequiredService<TView>();
        window.DataContext = vm;

        WindowThemeNormalize(window);

        // Запоминаем пару, чтобы потом закрыть
        _openedWindows[vm] = window;

        // Если окно закрывается иным способом — убираем из словаря
        window.Closed += (_, _) => _openedWindows.Remove(vm);
        
        window.Show();
    }

    public void CloseWindow(ViewModelBase viewModel)
    {
        if (_openedWindows.TryGetValue(viewModel, out var window))
        {
            window.Close();
            _openedWindows.Remove(viewModel);
        }
    }

    private void WindowThemeNormalize(SukiWindow window)
    {
        window.BackgroundStyle = SukiBackgroundStyle.Gradient; // Базовый градиент

        var theme = SukiTheme.GetInstance();
        if (!theme.ActiveBaseTheme.Equals(ThemeVariant.Dark))
            theme.SwitchBaseTheme(); // Темная тема
    }
}
