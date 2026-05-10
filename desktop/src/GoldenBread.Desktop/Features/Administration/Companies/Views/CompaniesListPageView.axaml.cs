using Avalonia.Controls;
using GoldenBread.Desktop.Features.Administration.Companies.ViewModels;
using GoldenBread.Desktop.Features.Administration.Users.ViewModels;
using GoldenBread.Desktop.Features.Menu;
using GoldenBread.Desktop.UI.Helpers;
using ReactiveUI;

namespace GoldenBread.Desktop.Features.Administration.Companies.Views;

public partial class CompaniesListPageView : UserControl
{
    public CompaniesListPageView()
    {
        InitializeComponent();
    }

    private void DataGrid_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (DataContext is CompaniesListPageViewModel vm && vm.SelectedItem != null)
        {
            vm.ShowDetailCommand.Execute();
        }
    }
}