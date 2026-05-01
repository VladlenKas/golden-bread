using GoldenBread.Desktop.Features.References.Employees.Models;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;

namespace GoldenBread.Desktop.Features.References.Employees.ViewModels;

public partial class EmployeesPageViewModel : StackPageViewModel
{
    private readonly EmployeesListPageViewModel _listPage;
    private readonly PageFactory _factory;

    public EmployeesPageViewModel(
        EmployeesListPageViewModel listPage,
        PageFactory factory)
    {
        _listPage = listPage;
        _factory = factory;

        _listPage.EditCommand.Subscribe(item => ShowEdit(item));
        _listPage.AddCommand.Subscribe(_ => ShowEdit(null));
    }

    private void ShowList()
    {
        _listPage.RefreshCommand.Execute();
        NavigateTo(_listPage);
    }

    private void ShowEdit(EmployeeListItem? employee)
    {
        var editPage = _factory.Create<EditableEmployeePageViewModel>();
        editPage.Title = employee == null ? "Добавление" : "Редактирование";
        editPage.SelectedItem = employee;

        editPage.SaveCommand.Subscribe(_ => ShowList());
        editPage.GoBackCommand.Subscribe(_ => ShowList());

        NavigateTo(editPage);
    }

    protected override void OnActivated() => ShowList();
    protected override void OnDeactivated() => ShowList();
}