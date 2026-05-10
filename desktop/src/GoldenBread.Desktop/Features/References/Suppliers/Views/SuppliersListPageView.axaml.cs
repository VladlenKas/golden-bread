using Avalonia.Controls;
using GoldenBread.Desktop.Features.References.Suppliers.ViewModels;

namespace GoldenBread.Desktop.Features.References.Suppliers.Views;

public partial class SuppliersListPageView : UserControl
{
    public SuppliersListPageView()
    {
        InitializeComponent();
    }

    private void DataGrid_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (DataContext is SuppliersListPageViewModel vm && vm.SelectedItem != null)
        {
            vm.ShowDetailCommand.Execute();
        }
    }
}