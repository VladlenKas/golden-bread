using Avalonia.Controls;
using GoldenBread.Desktop.Features.Administration.Companies.ViewModels;
using GoldenBread.Desktop.Features.Administration.Users.ViewModels;

namespace GoldenBread.Desktop.Features.Administration.Users.Views;

public partial class UsersListPageView : UserControl
{
    public UsersListPageView()
    {
        InitializeComponent();
    }

    private void DataGrid_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (DataContext is UsersListPageViewModel vm && vm.SelectedItem != null)
        {
            vm.ShowDetailCommand.Execute();
        }
    }
}