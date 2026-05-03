using DynamicData;
using GoldenBread.Desktop.Features.References.Employees.Models;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using SukiUI.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;

namespace GoldenBread.Desktop.Features.References.Employees.ViewModels;

public partial class EmployeesListPageViewModel : PageViewModel, ISukiStackPageTitleProvider
{
    private readonly IEmployeesApi _api;
    private readonly DialogService _dialogService;
    private readonly ToastService _toastService;
    private readonly SourceList<EmployeeListItem> _sourceList = new();

    [Reactive] private bool _isBusy;
    [Reactive] public bool _isEmpty;
    [Reactive] private string _searchText = string.Empty;
    [Reactive] public EmployeeListItem? _selectedItem;

    public string Title { get; set; } = ConstantMessages.EmployeesTitlePage;
    public ReadOnlyObservableCollection<EmployeeListItem> FilteredItems { get; }

    public EmployeesListPageViewModel(
        IEmployeesApi api,
        DialogService dialogService,
        ToastService toastService)
    {
        _api = api;
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

    // Фильтрация
    private static Func<EmployeeListItem, bool> SearchFilter(string? search)
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

            var response = await _api.GetAll();
            if (!response.IsSuccessStatusCode || response.Content == null)
                return;

            var data = response.Content;

            _sourceList.Clear();
            foreach (var item in data.EmployeesList)
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
    private async Task DeleteAsync()
    {
        var tcs = _dialogService.ShowQustion(ConstantMessages.EmployeeDismissConfirmDialog);

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

            var response = await _api.Delete(SelectedItem!.EmployeeId);

            if (response.IsSuccessStatusCode)
            {
                _sourceList.Remove(SelectedItem);
                _toastService.ShowSuccess(ConstantMessages.EmployeeDismissedToast);
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

    [ReactiveCommand] // Оповещение оркестартора
    private async Task AddAsync() { }

    [ReactiveCommand] // Оповещение оркестартора
    private async Task<EmployeeListItem?> EditAsync(EmployeeListItem? selectedItem) => selectedItem;
}