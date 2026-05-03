using GoldenBread.Desktop.Features.References.Suppliers.Models;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;
using System.Reactive.Linq;

namespace GoldenBread.Desktop.Features.References.Suppliers.ViewModels;

public partial class SuppliersHostPageViewModel : HostPageViewModel
{
    private readonly SuppliersListPageViewModel _listPage;
    private readonly PageFactory _factory;

    public SuppliersHostPageViewModel(PageFactory factory)
    {
        var listPage = factory.GetPage<SuppliersListPageViewModel>();

        _factory = factory;
        _listPage = listPage;

        _listPage.EditCommand.Subscribe(item => ShowEditor(item));
        _listPage.AddCommand.Subscribe(_ => ShowEditor(null));
    }

    private void ShowList()
    {
        _listPage.RefreshCommand.Execute();
        NavigateTo(_listPage);
    }

    private void ShowEditor(SupplierListItem? supplier)
    {
        var editPage = _factory.GetPage<SupplierEditorPageViewModel>();
        editPage.SelectedItem = supplier;

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