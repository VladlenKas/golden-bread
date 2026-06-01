using DynamicData;
using GoldenBread.Desktop.Features.Administration.Companies.Models;
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
using static GoldenBread.Desktop.UI.Helpers.LocalizedVerificationStatuses;

namespace GoldenBread.Desktop.Features.Administration.Companies.ViewModels;

public partial class CompaniesListPageViewModel : PageViewModel, ISukiStackPageTitleProvider
{
    private readonly ICompaniesApi _api;
    private readonly IAccountApi _accountApi;
    private readonly CurrentUserStore _userStore;
    private readonly DialogService _dialogService;
    private readonly ToastService _toastService;
    private readonly SourceList<CompanyListItem> _sourceList = new();

    [Reactive] private bool _isBusy;
    [Reactive] public bool _isEmpty = true;
    [Reactive] private string _searchText = string.Empty;
    [Reactive] public CompanyListItem? _selectedItem;
    [Reactive] public StatusesFilterOption? _selectedStatusFilter = LocalizedVerificationStatuses.StatusesFilters[0];

    public List<StatusesFilterOption> StatusFilterOptions => LocalizedVerificationStatuses.StatusesFilters;
    public ReadOnlyObservableCollection<CompanyListItem> FilteredItems { get; }
    public string Title { get; set; } = "Список компаний";

    public CompaniesListPageViewModel(
        ICompaniesApi api,
        IAccountApi accountApi,
        CurrentUserStore userStore,
        DialogService dialogService,
        ToastService toastService)
    {
        _api = api;
        _accountApi = accountApi;
        _userStore = userStore;
        _dialogService = dialogService;
        _toastService = toastService;

        var filter = this.WhenAnyValue(
            x => x.SearchText,
            x => x.SelectedStatusFilter)
            .DistinctUntilChanged()
            .Select(tuple => CombinedFilter(
                tuple.Item1,
                tuple.Item2?.Value));

        _sourceList.Connect()
            .Filter(filter)
            .Bind(out var filtered)
            .Subscribe(_ => IsEmpty = filtered.Count == 0);

        FilteredItems = filtered;
    }

    private static Func<CompanyListItem, bool> CombinedFilter(
        string? search,
        VerificationStatus? status)
    {
        return item =>
        {
            if (!string.IsNullOrWhiteSpace(search) &&
                !item.SearchText.Contains(search, StringComparison.InvariantCultureIgnoreCase))
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
        SelectedStatusFilter = StatusFilterOptions[0];
    }

    [ReactiveCommand]
    private async Task RefreshAsync()
    {
        try
        {
            IsBusy = true;

            var response = await _api.GetAll();
            if (!response.IsSuccessStatusCode || response.Content == null)
                return;

            var data = response.Content;

            _sourceList.Clear();
            foreach (var item in data.CompaniesList)
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
    private async Task SetStatusPendingAsync(CompanyListItem? item) =>
        await ChangeStatusAsync(item, VerificationStatus.Pending);

    [ReactiveCommand]
    private async Task SetStatusApprovedAsync(CompanyListItem? item) =>
        await ChangeStatusAsync(item, VerificationStatus.Approved);

    [ReactiveCommand]
    private async Task SetStatusRejectedAsync(CompanyListItem? item) =>
        await ChangeStatusAsync(item, VerificationStatus.Rejected);

    [ReactiveCommand]
    private async Task SetStatusSuspendedAsync(CompanyListItem? item) =>
        await ChangeStatusAsync(item, VerificationStatus.Suspended);

    private async Task ChangeStatusAsync(CompanyListItem? item, VerificationStatus status)
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
            var response = await _accountApi.UpdateStatus(request);

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
    private async Task<CompanyListItem?> ChangeEmailAsync(CompanyListItem? item)
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
    private async Task<CompanyListItem?> ChangePasswordAsync(CompanyListItem? item)
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
    private async Task DeleteAsync(CompanyListItem? item)
    {
        if (item == null)
        {
            _toastService.ShowInfo(ConstantMessages.EmptySelectedItem);
            return;
        }
        else if (!item.CanDelete)
        {
            _toastService.ShowWarning(ConstantMessages.CompanyCannotBeDeleted);
            return;
        }

        var tcs = _dialogService.ShowWarningQuestion(ConstantMessages.CompanyDeleteConfirmDialog);

        bool confirmed = await tcs.Task;

        if (!confirmed)
            return;

        IsBusy = true;
        try
        {
            var response = await _accountApi.Delete(item.AccountId);

            if (response.IsSuccessStatusCode)
            {
                _sourceList.Remove(item);
                _toastService.ShowSuccess(ConstantMessages.DeletedToast);
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
    private async Task<CompanyListItem?> EditAsync(CompanyListItem? selectedItem) => selectedItem;

    [ReactiveCommand]
    private async Task ShowDetail()
    {
        var vm = DetailDialogFactory.FromCompany(SelectedItem!);
        _dialogService.ShowDetailViewModel(vm);
    }
}
