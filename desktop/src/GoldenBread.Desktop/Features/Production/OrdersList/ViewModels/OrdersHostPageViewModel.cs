using GoldenBread.Desktop.Features.Production.OrdersList.Models;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;
using System.Reactive.Linq;

namespace GoldenBread.Desktop.Features.Production.OrdersList.ViewModels;

public partial class OrdersHostPageViewModel : HostPageViewModel
{
    private readonly OrdersListPageViewModel _listPage;
    private readonly PageFactory _factory;

    public OrdersHostPageViewModel(PageFactory factory)
    {
        _factory = factory;
        _listPage = factory.GetPage <OrdersListPageViewModel> ();

        _listPage.AddCommand.Subscribe(_ => ShowEditor(null));
    }

    private void ShowList()
    {
        _listPage.RefreshCommand.Execute();
        NavigateTo(_listPage);
    }

    private void ShowEditor(KanbanItem? item)
    {
        var editPage = _factory.GetPage<OrderEditorPageViewModel>();

        editPage.SaveCommand
            .Where(action => action)
            .Take(1)
            .Subscribe(_ =>
            {
                ShowList();
            });

        editPage.GoBackCommand
            .Take(1)
            .Subscribe(_ => ShowList());

        NavigateTo(editPage);
    }

    protected override void OnActivated()
    {
        _listPage.Permissions = this.Permissions;
        ShowList();
    }
    protected override void OnDeactivated() => ShowList();
}