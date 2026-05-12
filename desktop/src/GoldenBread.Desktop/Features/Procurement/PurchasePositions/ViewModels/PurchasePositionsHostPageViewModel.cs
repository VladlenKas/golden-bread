using GoldenBread.Desktop.Features.Procurement.PurchasePositions.Models;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;
using System.Reactive.Linq;

namespace GoldenBread.Desktop.Features.Procurement.PurchasePositions.ViewModels;

public partial class PurchasePositionsHostPageViewModel : HostPageViewModel
{
    private readonly PurchasePositionsListPageViewModel _listPage;
    private readonly PageFactory _factory;

    public PurchasePositionsHostPageViewModel(PageFactory factory)
    {
        var listPage = factory.GetPage<PurchasePositionsListPageViewModel>();

        _factory = factory;
        _listPage = listPage;

        _listPage.EditCommand.Subscribe(item => ShowEditor(item));
        _listPage.AddCommand.Subscribe(_ => ShowEditor(null));
        _listPage.ShowEditIngredientDialogCommand.Where(action => action).Subscribe(_ => ShowList());
        _listPage.ShowDeleteIngredientDialogCommand.Where(action => action).Subscribe(_ => ShowList());
    }

    private void ShowList()
    {
        _listPage.RefreshCommand.Execute();
        NavigateTo(_listPage);
    }

    private void ShowEditor(SupplierIngredientListItem? item)
    {
        var editPage = _factory.GetPage<PurchasePositionEditorPageViewModel>();
        editPage.SelectedItem = item;

        editPage.SaveCommand
            .Where(action => action)
            .Take(1)
            .Subscribe(_ => ShowList());

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