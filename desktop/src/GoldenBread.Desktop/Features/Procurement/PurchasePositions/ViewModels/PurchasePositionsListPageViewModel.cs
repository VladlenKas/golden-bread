using DynamicData;
using GoldenBread.Desktop.Features.Common;
using GoldenBread.Desktop.Features.Procurement.PurchasePositions.Models;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Helpers;
using GoldenBread.Desktop.UI.Services;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using SukiUI.Controls;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Tmds.DBus.Protocol;
using static GoldenBread.Desktop.UI.Helpers.LocalizedIngredientUnits;

namespace GoldenBread.Desktop.Features.Procurement.PurchasePositions.ViewModels;

public partial class PurchasePositionsListPageViewModel : PageViewModel, ISukiStackPageTitleProvider
{
    private readonly ISupplierIngredientsApi _api;
    private readonly IIngredientsApi _ingredientsApi;
    private readonly DialogService _dialogService;
    private readonly ToastService _toastService;
    private readonly SourceList<SupplierIngredientListItem> _sourceList = new();

    [Reactive] private bool _isBusy;
    [Reactive] public bool _isEmpty = true;
    [Reactive] private string _searchText = string.Empty;
    [Reactive] public SupplierIngredientListItem? _selectedItem;
    [Reactive] public UnitFilterOption? _selectedUnitFilter = LocalizedIngredientUnits.UnitsFilters[0];

    public List<UnitFilterOption> UnitFilterOptions => LocalizedIngredientUnits.UnitsFilters;
    public string Title { get; set; } = ConstantMessages.HostTitlePage;
    public ReadOnlyObservableCollection<SupplierIngredientListItem> FilteredItems { get; }

    public PurchasePositionsListPageViewModel(
        ISupplierIngredientsApi api,
        IIngredientsApi ingredientsApi,
        DialogService dialogService,
        ToastService toastService)
    {
        _api = api;
        _ingredientsApi = ingredientsApi;
        _dialogService = dialogService;
        _toastService = toastService;

        var filter = this.WhenAnyValue(
            x => x.SearchText,
            x => x.SelectedUnitFilter)
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

    private static Func<SupplierIngredientListItem, bool> CombinedFilter(
        string? search,
        IngredientUnit? unit)
    {
        return item =>
        {
            if (!string.IsNullOrWhiteSpace(search) &&
                !item.SearchText.Contains(search, StringComparison.InvariantCultureIgnoreCase))
                return false;

            if (unit.HasValue && item.Unit != unit.Value)
                return false;

            return true;
        };
    }

    [ReactiveCommand]
    private void ResetFilters()
    {
        SearchText = string.Empty;
        SelectedUnitFilter = UnitFilterOptions[0];
    }

    [ReactiveCommand]
    private async Task RefreshAsync()
    {
        try
        {
            IsBusy = true;
            var response = await _api.GetList();
            if (!response.IsSuccessStatusCode || response.Content == null)
                return;

            _sourceList.Clear();
            foreach (var item in response.Content.SupplierIngredientsList)
                _sourceList.Add(item);
        }
        catch
        {
            _dialogService.ShowInfo(ConstantMessages.ExceptionDialog);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [ReactiveCommand]
    private async Task DeleteAsync(SupplierIngredientListItem? item)
    {
        if (item == null)
        {
            _toastService.ShowError(ConstantMessages.EmptySelectedItem);
            return;
        }

        var message = string.Format(
            ConstantMessages.SupplierIngredientDeleteConfirmDialog,
            item.QuantityUnitInBatchesFormatted,
            item.QuantityBatches);

        var tcs = _dialogService.ShowWarningQuestion(message);

        bool confirmed = await tcs.Task;
        if (!confirmed) return;

        IsBusy = true;
        try
        {
            var response = await _api.Delete(item.SupplierIngredientId);
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
    private async Task ShowCreateIngredientDialog()
    {
        var vm = new IngredientCreateDialogViewModel(_ingredientsApi, _toastService);
        var tcs = _dialogService.ShowDialogAsync(vm, "Новый ингредиент");

        bool confirmed = await tcs.Task;
        if (!confirmed) return;

        _toastService.ShowSuccess(ConstantMessages.CreatedToast);
    }

    [ReactiveCommand]
    private async Task<bool> ShowEditIngredientDialog()
    {
        var ingredients = await _ingredientsApi.GetAll();
        if (!ingredients.IsSuccessStatusCode) return false;

        var vm = new IngredientEditDialogViewModel(
            _ingredientsApi,
            _toastService,
            ingredients.Content!.IngredientsList);

        var tcs = _dialogService.ShowDialogAsync(vm, "Новый ингредиент");

        bool confirmed = await tcs.Task;
        if (!confirmed) return false;

        _toastService.ShowSuccess(ConstantMessages.UpdatedToast);
        return true;
    }

    [ReactiveCommand]
    private async Task<bool> ShowDeleteIngredientDialog()
    {
        var ingredients = await _ingredientsApi.GetAll();
        if (!ingredients.IsSuccessStatusCode) return false;

        var vm = new IngredientDeleteDialogViewModel(
            _ingredientsApi,
            _toastService,
            ingredients.Content!.IngredientsList);

        var tcs = _dialogService.ShowDialogAsync(vm, "Удалить ингредиент");

        bool confirmed = await tcs.Task;
        if (!confirmed) return false;

        _toastService.ShowSuccess(ConstantMessages.DeletedToast);
        return true;
    }

    [ReactiveCommand]
    private async Task ShowDetail()
    {
        var vm = DetailDialogFactory.FromPurchasePosition(SelectedItem!);
        _dialogService.ShowDetailViewModel(vm);
    }

    [ReactiveCommand]
    private async Task AddAsync() { }

    [ReactiveCommand]
    private async Task<SupplierIngredientListItem?> EditAsync(SupplierIngredientListItem? selectedItem) => selectedItem;
}