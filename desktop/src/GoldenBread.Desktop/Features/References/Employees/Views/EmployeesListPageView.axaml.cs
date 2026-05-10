using Avalonia.Controls;
using GoldenBread.Desktop.Features.Administration.Users.ViewModels;
using GoldenBread.Desktop.Features.References.Employees.ViewModels;

namespace GoldenBread.Desktop.Features.References.Employees.Views;

public partial class EmployeesListPageView : UserControl
{
    public EmployeesListPageView()
    {
        InitializeComponent();
    }

    private void DataGrid_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (DataContext is EmployeesListPageViewModel vm && vm.SelectedItem != null)
        {
            vm.ShowDetailCommand.Execute();
        }
    }
}