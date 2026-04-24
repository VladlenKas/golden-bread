using Avalonia.Controls;
using GoldenBread.Desktop.UI.Common;
using SukiUI.Controls;

namespace GoldenBread.Desktop.UI.Services.Windows;

public interface IWindowService
{
    /// <summary>
    /// Регистрирует новое окно с переданной ViewModel
    /// </summary>
    void RegisterWindow(ViewModelBase viewModel, SukiWindow window);

    /// <summary>
    /// Открывает новое окно для переданной ViewModel
    /// </summary>
    void ShowWindow<TView, TViewModel>(TViewModel? viewModel = null)
        where TView : SukiWindow
        where TViewModel : ViewModelBase;

    /// <summary>
    /// Закрывает окно, в котором отображается данная ViewModel
    /// </summary>
    void CloseWindow(ViewModelBase viewModel);
}