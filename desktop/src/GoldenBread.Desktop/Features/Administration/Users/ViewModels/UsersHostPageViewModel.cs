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

    protected override void OnActivated()
    {
        _listPage.Permissions = this.Permissions;
        ShowList();
    }

    protected override void OnDeactivated() => ShowList();
}
