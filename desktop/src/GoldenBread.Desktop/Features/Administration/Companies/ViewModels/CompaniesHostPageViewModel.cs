using GoldenBread.Desktop.Features.Administration.Companies.Models;
using GoldenBread.Desktop.Features.Administration.Users.Models;
using GoldenBread.Desktop.Features.Administration.Users.ViewModels;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;
using System.Reactive.Linq;

namespace GoldenBread.Desktop.Features.Administration.Companies.ViewModels;

public partial class CompaniesHostPageViewModel : HostPageViewModel
{
    private readonly CompaniesListPageViewModel _listPage;
    private readonly PageFactory _factory;

    public CompaniesHostPageViewModel(PageFactory factory)
    {
        var listPage = factory.GetPage<CompaniesListPageViewModel>();

        _factory = factory;
        _listPage = listPage;

        _listPage.EditCommand.Subscribe(item => ShowEditor(item));
        _listPage.AddCommand.Subscribe(_ => ShowEditor(null));
        _listPage.ChangeEmailCommand.Subscribe(item => ShowChangeEmail(item));
        _listPage.ChangePasswordCommand.Subscribe(item => ShowChangePassword(item));
    }

    private void ShowList()
    {
        _listPage.RefreshCommand.Execute();
        NavigateTo(_listPage);
    }

    private void ShowEditor(CompanyListItem? company)
    {
        var editPage = _factory.GetPage<CompanyEditorPageViewModel>();
        editPage.SelectedItem = company;

        editPage.SaveCommand
            .Where(action => action)
            .Take(1)
            .Subscribe(_ => ShowList());

        editPage.GoBackCommand
            .Take(1)
            .Subscribe(_ => ShowList());

        NavigateTo(editPage);
    }

    private void ShowChangeEmail(CompanyListItem? user)
    {
        if (user == null) return;

        var page = _factory.GetPage<ChangeEmailPageViewModel>();
        page.AccountId = user.AccountId;

        page.SaveCommand
            .Where(action => action)
            .Take(1)
            .Subscribe(_ => ShowList());

        page.GoBackCommand
            .Take(1)
            .Subscribe(_ => ShowList());

        NavigateTo(page);
    }

    private void ShowChangePassword(CompanyListItem? user)
    {
        if (user == null) return;

        var page = _factory.GetPage<ChangePasswordPageViewModel>();
        page.AccountId = user.AccountId;

        page.SaveCommand
            .Where(action => action)
            .Take(1)
            .Subscribe(_ => ShowList());

        page.GoBackCommand
            .Take(1)
            .Subscribe(_ => ShowList());

        NavigateTo(page);
    }

    protected override void OnActivated()
    {
        _listPage.Permissions = this.Permissions;
        ShowList();
    }

    protected override void OnDeactivated() => ShowList();
}
