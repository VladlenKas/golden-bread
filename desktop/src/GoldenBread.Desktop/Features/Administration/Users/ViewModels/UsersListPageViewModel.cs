using DynamicData;
using GoldenBread.Desktop.Features.Administration.Users.Models;
using GoldenBread.Desktop.Features.Common;
using GoldenBread.Desktop.Features.Common.Account;
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
using static GoldenBread.Desktop.UI.Helpers.LocalizedRoles;
using static GoldenBread.Desktop.UI.Helpers.LocalizedVerificationStatuses;

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
    [Reactive] public bool _isEmpty = true;
    [Reactive] private string _searchText = string.Empty;
    [Reactive] public UserListItem? _selectedItem;
    [Reactive] public RoleFilterOption? _selectedRoleFilter = LocalizedRoles.RolesFilters[0];
    [Reactive] public StatusesFilterOption? _selectedStatusFilter = LocalizedVerificationStatuses.StatusesFilters[0];

    public List<StatusesFilterOption> StatusFilterOptions => LocalizedVerificationStatuses.StatusesFilters;
    public List<RoleFilterOption> RoleFilterOptions => LocalizedRoles.RolesFilters;
    public ReadOnlyObservableCollection<UserListItem> FilteredItems { get; }
    public string Title { get; set; } = ConstantMessages.HostTitlePage;

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

        var filter = this.WhenAnyValue(
            x => x.SearchText,
            x => x.SelectedRoleFilter,
            x => x.SelectedStatusFilter)
            .DistinctUntilChanged()
            .Select(tuple => CombinedFilter(
                tuple.Item1, 
                tuple.Item2?.Value, 
                tuple.Item3?.Value));

        _sourceList.Connect()
            .Filter(filter)
            .Bind(out var filtered)
            .Subscribe(_ => IsEmpty = filtered.Count == 0);

        FilteredItems = filtered;
    }

    private static Func<UserListItem, bool> CombinedFilter(
        string? search,
        UserRole? role,
        VerificationStatus? status)
    {
        return item =>
        {
            if (!string.IsNullOrWhiteSpace(search) &&
                !item.SearchText.Contains(search, StringComparison.InvariantCultureIgnoreCase))
                return false;

            if (role.HasValue && item.Role != role.Value)  
                return false;

            if (status.HasValue && item.VerificationStatus != status.Value)
                return false;

            return true;
        };
    }

    [ReactiveCommand]
    private void ResetFilters()
    {
        SearchText = string.Empty;
        SelectedRoleFilter = RoleFilterOptions[0];
        SelectedStatusFilter = StatusFilterOptions[0];
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
            _toastService.ShowInfo(ConstantMessages.EmptySelectedItem);
            return;
        }

        if (item.AccountId == _userStore.UserId)
        {
            _toastService.ShowWarning(ConstantMessages.SelfActionNotAllowed);
            return;
        }

        var tcs = _dialogService.ShowInfoQuestion(ConstantMessages.UpdateAccountStatusConfirmDialog);

        bool confirmed = await tcs.Task;

        if (!confirmed)
            return;

        IsBusy = true;
        try
        {
            if (SelectedItem == null)
            {
                _toastService.ShowInfo(ConstantMessages.EmptySelectedItem);
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
            _toastService.ShowInfo(ConstantMessages.EmptySelectedItem);
            return null;
        }

        if (item.AccountId == _userStore.UserId)
        {
            _toastService.ShowWarning(ConstantMessages.SelfActionNotAllowed);
            return null;
        }

        return item;
    }

    [ReactiveCommand]
    private async Task<UserListItem?> ChangePasswordAsync(UserListItem? item)
    {
        if (item == null)
        {
            _toastService.ShowInfo(ConstantMessages.EmptySelectedItem);
            return null;
        }

        if (item.AccountId == _userStore.UserId)
        {
            _toastService.ShowWarning(ConstantMessages.SelfActionNotAllowed);
            return null;
        }

        return item;
    }

    [ReactiveCommand]
    private async Task DeleteAsync(UserListItem? item)
    {
        if (item == null)
        {
            _toastService.ShowInfo(ConstantMessages.EmptySelectedItem);
            return;
        }

        if (item.AccountId == _userStore.UserId)
        {
            _toastService.ShowWarning(ConstantMessages.SelfActionNotAllowed);
            return;
        }

        var tcs = _dialogService.ShowWarningQuestion(ConstantMessages.EmployeeDismissConfirmDialog);

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
                _toastService.ShowSuccess(ConstantMessages.EmployeeDismissedToast);
            }
            else
            {
                var msg = response.Error != null
                        ? GoldenBreadApiClient.GetErrorMessage(response.Error)
                        : null;

                _toastService.ShowError(msg);
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
    private async Task ShowDetail()
    {
        var vm = DetailDialogFactory.FromUser(SelectedItem!);
        _dialogService.ShowDetailViewModel(vm);
    }

    [ReactiveCommand]
    private async Task AddAsync() { }

    [ReactiveCommand]
    private async Task<UserListItem?> EditAsync(UserListItem? selectedItem) => selectedItem;
}