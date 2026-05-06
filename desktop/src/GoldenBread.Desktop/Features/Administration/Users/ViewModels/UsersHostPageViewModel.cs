using GoldenBread.Desktop.Features.Administration.Users.Models;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;
using System.Reactive.Linq;

namespace GoldenBread.Desktop.Features.Administration.Users.ViewModels;

public partial class UsersHostPageViewModel : HostPageViewModel
{
    private readonly UsersListPageViewModel _listPage;
    private readonly PageFactory _factory;

    public UsersHostPageViewModel(PageFactory factory)
    {
        var listPage = factory.GetPage<UsersListPageViewModel>();

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

    private void ShowEditor(UserListItem? user)
    {
        var editPage = _factory.GetPage<UserEditorPageViewModel>();
        editPage.SelectedItem = user;

        editPage.SaveCommand
            .Where(action => action)
            .Take(1)
            .Subscribe(_ => ShowList());

        editPage.GoBackCommand
            .Take(1)
            .Subscribe(_ => ShowList());

        NavigateTo(editPage);
    }

    private void ShowChangeEmail(UserListItem? user)
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

    private void ShowChangePassword(UserListItem? user)
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
