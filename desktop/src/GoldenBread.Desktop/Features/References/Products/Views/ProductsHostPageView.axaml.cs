using Avalonia.Controls;
using GoldenBread.Desktop.Features.References.Products.ViewModels;
using ReactiveUI.Avalonia;

namespace GoldenBread.Desktop.Features.References.Products.Views;

public partial class ProductsHostPageView : ReactiveUserControl<ProductsHostPageViewModel>
{
    public ProductsHostPageView()
    {
        InitializeComponent();
    }
}