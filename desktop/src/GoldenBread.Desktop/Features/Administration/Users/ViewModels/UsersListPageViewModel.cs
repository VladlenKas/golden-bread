using DynamicData;
using GoldenBread.Desktop.Features.Administration.Users.Models;
using GoldenBread.Desktop.Features.Common.Models;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.Infrastructure.Auth;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Helpers;
using GoldenBread.Desktop.UI.Services;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using SukiUI.Controls;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace GoldenBread.Desktop.Features.Administration.Users.ViewModels;

public partial class UsersListPageViewModel : PageViewModel, ISukiStackPageTitleProvider
{
    private readonly IUsersApi _usersApi;
    private readonly CurrentUserStore _userStore;
    private readonly IAccountApi _accountsApi;
    private readonly DialogService _dialogService;
    private readonly ToastService _toastService;
    private readonly SourceList<UserListItem> _sourceList = new();

    [Reactive] private bool _isBusy;
    [Reactive] public bool _isEmpty;
    [Reactive] private string _searchText = string.Empty;
    [Reactive] public UserListItem? _selectedItem;

    public string Title { get; set; } = ConstantMessages.HostTitlePage;
    public ReadOnlyObservableCollection<UserListItem> FilteredItems { get; }

    public UsersListPageViewModel(
        IUsersApi usersApi,
        CurrentUserStore userStore,
        IAccountApi accountsApi,
        DialogService dialogService,
        ToastService toastService)
    {
        _userStore = userStore;
        _usersApi = usersApi;
        _accountsApi = accountsApi;
        _dialogService = dialogService;
        _toastService = toastService;


        var filter = this.WhenAnyValue(x => x.SearchText)
            .DistinctUntilChanged()
            .Select(SearchFilter);

        _sourceList.Connect()
            .Filter(filter)
            .Bind(out var filtered)
            .Subscribe(_ =>
            {
                IsEmpty = filtered.Count == 0;
            });

        FilteredItems = filtered;
    }

    private static Func<UserListItem, bool> SearchFilter(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return _ => true;

        var lower = search.ToLowerInvariant();
        return item => item.SearchText.Contains(lower, StringComparison.InvariantCultureIgnoreCase);
    }

    [ReactiveCommand]
    private async Task RefreshAsync()
    {
        try
        {
            IsBusy = true;

            var response = await _usersApi.GetAll();
            if (!response.IsSuccessStatusCode || response.Content == null)
                return;

            var data = response.Content;

            _sourceList.Clear();
            foreach (var item in data.UsersList)
                _sourceList.Add(item);
        }
        catch (Exception)
        {
            _dialogService.ShowInfo(ConstantMessages.ExceptionDialog);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [ReactiveCommand]
    private async Task SetStatusPendingAsync(UserListItem? item) =>
        await ChangeStatusAsync(item, VerificationStatus.Pending);

    [ReactiveCommand]
    private async Task SetStatusApprovedAsync(UserListItem? item) =>
        await ChangeStatusAsync(item, VerificationStatus.Approved);

    [ReactiveCommand]
    private async Task SetStatusRejectedAsync(UserListItem? item) =>
        await ChangeStatusAsync(item, VerificationStatus.Rejected);

    [ReactiveCommand]
    private async Task SetStatusSuspendedAsync(UserListItem? item) =>
        await ChangeStatusAsync(item, VerificationStatus.Suspended);

    private async Task ChangeStatusAsync(UserListItem? item, VerificationStatus status)
    {
        if (item == null)
        {
            _toastService.ShowError(ConstantMessages.EmptySelectedItem);
            return;
        }

        if (item.AccountId == _userStore.UserId)
        {
            _toastService.ShowError(ConstantMessages.SelfActionNotAllowed);
            return;
        }

        var tcs = _dialogService.ShowInfoQustion(ConstantMessages.UpdateAccountStatusConfirmDialog);

        bool confirmed = await tcs.Task;

        if (!confirmed)
            return;

        IsBusy = true;
        try
        {
            if (SelectedItem == null)
            {
                _toastService.ShowError(ConstantMessages.EmptySelectedItem);
                return;
            }

            var request = new UpdateAccountStatusRequest(SelectedItem.AccountId, status);
            var response = await _accountsApi.UpdateStatus(request);

            if (response.IsSuccessStatusCode)
            {
                var index = _sourceList.Items.ToList().FindIndex(x => x.AccountId == SelectedItem.AccountId);
                if (index >= 0)
                {
                    var updatedItem = SelectedItem with { VerificationStatus = status };
                    _sourceList.Replace(SelectedItem, updatedItem);
                }

                _toastService.ShowSuccess(ConstantMessages.GetUpdateStatusMessage(status));
            }
            else
            {
                _toastService.ShowError();
            }
        }
        catch
        {
            _dialogService.ShowError();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [ReactiveCommand]
    private async Task<UserListItem?> ChangeEmailAsync(UserListItem? item)
    {
        if (item == null)
        {
            _toastService.ShowError(ConstantMessages.EmptySelectedItem);
            return null;
        }

        if (item.AccountId == _userStore.UserId)
        {
            _toastService.ShowError(ConstantMessages.SelfActionNotAllowed);
            return null;
        }

        return item;
    }

    [ReactiveCommand]
    private async Task<UserListItem?> ChangePasswordAsync(UserListItem? item)
    {
        if (item == null)
        {
            _toastService.ShowError(ConstantMessages.EmptySelectedItem);
            return null;
        }

        if (item.AccountId == _userStore.UserId)
        {
            _toastService.ShowError(ConstantMessages.SelfActionNotAllowed);
            return null;
        }

        return item;
    }

    [ReactiveCommand]
    private async Task DeleteAsync(UserListItem? item)
    {
        if (item == null)
        {
            _toastService.ShowError(ConstantMessages.EmptySelectedItem);
            return;
        }

        if (item.AccountId == _userStore.UserId)
        {
            _toastService.ShowError(ConstantMessages.SelfActionNotAllowed);
            return;
        }

        var tcs = _dialogService.ShowWarningQustion(ConstantMessages.UserDeleteConfirmDialog);

        bool confirmed = await tcs.Task;

        if (!confirmed)
            return;

        IsBusy = true;
        try
        {
            var response = await _accountsApi.Delete(SelectedItem!.AccountId);

            if (response.IsSuccessStatusCode)
            {
                _sourceList.Remove(SelectedItem);
                _toastService.ShowSuccess(ConstantMessages.UserDeletedToast);
            }
            else
            {
                _toastService.ShowError();
            }
        }
        catch
        {
            _dialogService.ShowError();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [ReactiveCommand]
    private async Task AddAsync() { }

    [ReactiveCommand]
    private async Task<UserListItem?> EditAsync(UserListItem? selectedItem) => selectedItem;
}