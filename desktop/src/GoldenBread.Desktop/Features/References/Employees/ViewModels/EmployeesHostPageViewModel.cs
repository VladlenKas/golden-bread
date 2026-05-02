using GoldenBread.Desktop.Features.References.Employees.Models;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;

namespace GoldenBread.Desktop.Features.References.Employees.ViewModels;

public partial class EmployeesHostPageViewModel : HostPageViewModel
{
    private readonly EmployeesListPageViewModel _listPage;
    private readonly PageFactory _factory;

    public EmployeesHostPageViewModel(PageFactory factory)
    {
        var listPage = factory.GetPage<EmployeesListPageViewModel>();

        _listPage = listPage;
        _factory = factory;

        _listPage.Permissions = this.Permissions;

        _listPage.EditCommand.Subscribe(item => ShowEditor(item));
        _listPage.AddCommand.Subscribe(_ => ShowEditor(null));
    }

    private void ShowList()
    {
        _listPage.RefreshCommand.Execute();
        NavigateTo(_listPage);
    }

    private void ShowEditor(EmployeeListItem? employee)
    {
        var editPage = _factory.GetPage<EmployeeEditorPageViewModel>();
        editPage.SelectedItem = employee;

        editPage.SaveCommand.Subscribe(_ => ShowList());
        editPage.GoBackCommand.Subscribe(_ => ShowList());

        NavigateTo(editPage);
    }

    protected override void OnActivated() => ShowList();
    protected override void OnDeactivated() => ShowList();
}