using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;

namespace GoldenBread.Desktop.Features.Production.EmployeeTasksList.ViewModels;

public partial class EmployeeTasksHostPageViewModel : HostPageViewModel
{
    private readonly EmployeeTasksListPageViewModel _listPage;
    private readonly PageFactory _factory;

    public EmployeeTasksHostPageViewModel(PageFactory factory)
    {
        _factory = factory;
        _listPage = factory.GetPage <EmployeeTasksListPageViewModel> ();
    }

    private void ShowList()
    {
        _listPage.RefreshCommand.Execute();
        NavigateTo(_listPage);
    }

    protected override void OnActivated()
    {
        _listPage.Permissions = this.Permissions;
        ShowList();
    }

    protected override void OnDeactivated() => ShowList();
}