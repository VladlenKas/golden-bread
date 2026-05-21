using GoldenBread.Desktop.Features.Production.OrdersList.ViewModels;
using ReactiveUI.Avalonia;

namespace GoldenBread.Desktop.Features.Production.OrdersList.Views;

public partial class OrdersHostPageView : ReactiveUserControl<OrdersHostPageViewModel>
{
    public OrdersHostPageView()
    {
        InitializeComponent();
    }
}