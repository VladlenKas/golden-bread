using GoldenBread.Desktop.Features.Administration.SystemUsers.Models;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;
using ReactiveUI;
using SukiUI.Controls;
using System.Diagnostics;
using ReactiveUI.SourceGenerators;
using GoldenBread.Desktop.Features.Administration.Users.Models;
using GoldenBread.Desktop.UI.Helpers;
using GoldenBread.Desktop.Infrastructure.Auth;
using GoldenBread.Desktop.Features.Common.Account;

namespace GoldenBread.Desktop.Features.Administration.Users.ViewModels;

public partial class UserEditorPageViewModel : PageViewModel, ISukiStackPageTitleProvider
{
    private readonly IUsersApi _api;
    private readonly ToastService _toastService;
    private readonly DialogService _dialogService;

    [Reactive] private UserForm? _itemEditable = new();
    [Reactive] private UserListItem? _selectedItem;
    [Reactive] private bool _isBusy;

    public Dictionary<UserRole, string> Roles => LocalizedRoles.Roles;
    public bool CanChangeRole { get; set; } = true;
    private UserForm? ItemEditableCache { get; set; }
    public string Title { get; set; } = ConstantMessages.CreateTitlePage;

    public UserEditorPageViewModel(
        IUsersApi api,
        CurrentUserStore userStore,
        DialogService dialogService,
        ToastService toastService)
    {
        _api = api;
        _toastService = toastService;
        _dialogService = dialogService;

        this.WhenAnyValue(x => x.SelectedItem)
            .Subscribe(async item =>
            {
                Title = item == null
                  ? ConstantMessages.CreateTitlePage
                  : ConstantMessages.EditorTitlePage;

                if (item != null)
                {
                    CanChangeRole = userStore.UserId != item.AccountId;
                    await LoadUserAsync(item.UserId);
                }
            });
    }

    [ReactiveCommand]
    public async Task<bool> SaveAsync()
    {
        if (ItemEditable!.HasErrors)
        {
            _toastService.ShowError(ItemEditable.GetFirstError());
            return false;
        }

        if (ItemEditableCache is not null &&
            ItemEditableCache.EqualsValues(ItemEditable))
        {
            _toastService.ShowInfo(ConstantMessages.NoChangesToast);
            return false;
        }

        IsBusy = true;
        try
        {
            if (ItemEditable.UserId == 0)
            {
                // Добавить проверку на пароль и email

                var request = new CreateUserCommand(ItemEditable.ToDto(), ItemEditable.Email!, ItemEditable.Password!);
                var response = await _api.Create(request);

                if (response.IsSuccessStatusCode)
                {
                    _toastService.ShowSuccess(ConstantMessages.CreatedToast);
                    return true;
                }
                else
                {
                    _toastService.ShowError();
                    return false;
                }
            }
            else
            {
                var dto = ItemEditable.ToDto();
                var response = await _api.Update(dto);

                if (response.IsSuccessStatusCode)
                {
                    _toastService.ShowSuccess(ConstantMessages.UpdatedToast);
                    return true;
                }
                else
                {
                    Debug.WriteLine(response.Error);
                    _toastService.ShowError();
                    return false;
                }
            }
        }
        catch
        {
            _dialogService.ShowError(ConstantMessages.ExceptionDialog);
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [ReactiveCommand]
    public async Task GoBackAsync() { }

    private async Task LoadUserAsync(int id)
    {
        IsBusy = true;
        try
        {
            var response = await _api.GetById(id);

            if (!response.IsSuccessStatusCode || response.Content == null)
            {
                _dialogService.ShowError(ConstantMessages.ExceptionDialog);
                return;
            }

            ItemEditable = UserForm.FromDto(response.Content);
            ItemEditableCache = ItemEditable.Clone();
        }
        finally
        {
            IsBusy = false;
        }
    }
}