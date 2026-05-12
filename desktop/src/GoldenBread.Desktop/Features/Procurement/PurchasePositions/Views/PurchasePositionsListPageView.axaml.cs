using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GoldenBread.Desktop.Features.Administration.Users.ViewModels;
using GoldenBread.Desktop.Features.Procurement.PurchasePositions.ViewModels;

namespace GoldenBread.Desktop.Features.Procurement.PurchasePositions.Views;

public partial class PurchasePositionsListPageView : UserControl
{
    public PurchasePositionsListPageView()
    {
        InitializeComponent();
    }

    private void DataGrid_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (DataContext is PurchasePositionsListPageViewModel vm && vm.SelectedItem != null)
        {
            vm.ShowDetailCommand.Execute();
        }
    }
}
