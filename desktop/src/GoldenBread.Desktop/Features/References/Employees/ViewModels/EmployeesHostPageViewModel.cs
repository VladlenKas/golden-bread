using GoldenBread.Desktop.Features.References.Employees.Models;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;
using System.Reactive.Linq;

namespace GoldenBread.Desktop.Features.References.Employees.ViewModels;

public partial class EmployeesHostPageViewModel : HostPageViewModel
{
    private readonly EmployeesListPageViewModel _listPage;
    private readonly PageFactory _factory;

    public EmployeesHostPageViewModel(PageFactory factory)
    {
        var listPage = factory.GetPage<EmployeesListPageViewModel>();

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

    private void ShowEditor(EmployeeListItem? employee)
    {
        var editPage = _factory.GetPage<EmployeeEditorPageViewModel>();
        editPage.SelectedItem = employee;

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