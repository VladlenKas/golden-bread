using Avalonia.Controls;
using GoldenBread.Desktop.Features.References.Products.Models;
using GoldenBread.Desktop.Features.References.Products.ViewModels;

namespace GoldenBread.Desktop.Features.References.Products.Views;

public partial class ProductsListPageView : UserControl
{
    public ProductsListPageView()
    {
        InitializeComponent();
    }

    private void DataGrid_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (sender is Control { DataContext: ProductListItem item } &&
            DataContext is ProductsListPageViewModel vm)
        {
            vm.ShowDetailCommand.Execute(item).Subscribe();
        }
    }
}