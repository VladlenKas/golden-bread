using Avalonia.Controls;
using Avalonia.Interactivity;
using GoldenBread.Desktop.UI.Helpers;
using SukiUI.Controls;

namespace GoldenBread.Desktop.Features.Menu;

public partial class MenuWindowView : SukiWindow
{
    public MenuWindowView()
    {
        InitializeComponent();
    }

    private void ThemeMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MenuWindowViewModel vm) return;
        if (e.Source is not MenuItem { DataContext: LocalizedTheme localized }) return;

        vm.ChangeTheme(localized.Theme);
    }

    private void BackgroundMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MenuWindowViewModel vm) return;
        if (e.Source is not MenuItem { DataContext: LocalizedBackground localized }) return;

        vm.BackgroundStyle = localized.Style;
    }

    private void LogoutMenuItem_Tapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (DataContext is not MenuWindowViewModel vm) return;

        vm.LogoutCommand.Execute();
    }
}