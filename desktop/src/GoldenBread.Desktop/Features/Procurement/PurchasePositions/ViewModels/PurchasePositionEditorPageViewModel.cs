using GoldenBread.Desktop.Features.Common;
using GoldenBread.Desktop.Features.Procurement.PurchasePositions.Models;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using SukiUI.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reactive.Linq;
using static GoldenBread.Desktop.UI.Helpers.LocalizedIngredientUnits;

namespace GoldenBread.Desktop.Features.Procurement.PurchasePositions.ViewModels;

public partial class PurchasePositionEditorPageViewModel : PageViewModel, ISukiStackPageTitleProvider
{
    private readonly ISupplierIngredientsApi _api;
    private readonly ISuppliersApi _suppliersApi;
    private readonly IIngredientsApi _ingredientsApi;
    private readonly ToastService _toastService;
    private readonly DialogService _dialogService;

    [Reactive] private bool _isBusy;
    [Reactive] private bool _unitComboBoxIsEnabled = false;
    [Reactive] private string _unitComboBoxPlaceholder = string.Empty;
    [Reactive] private SupplierIngredientForm _itemEditable = new();
    [Reactive] private SupplierIngredientListItem? _selectedItem;
    [Reactive] private List<ItemsAutoCompleteBox> _supplierAutocompleteItems = new();
    [Reactive] private List<IngredientAutoCompleteItem> _ingredientsAutocompleteItems = new();
    [Reactive] private ObservableCollection<UnitFilterOption> _filteredUnits = new();
    [Reactive] private UnitFilterOption? _selectedUnitItem;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.IngredientRequiredValidation)]
    IngredientAutoCompleteItem? _selectedIngredientItem;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.SupplierRequiredValidation)]
    ItemsAutoCompleteBox? _selectedSupplierItem;

    private SupplierIngredientForm? ItemEditableCache { get; set; } = null;
    public string Title { get; set; } = ConstantMessages.CreateTitlePage;

    public PurchasePositionEditorPageViewModel(
        ISupplierIngredientsApi api,
        ISuppliersApi suppliersApi,
        IIngredientsApi ingredientsApi,
        DialogService dialogService,
        ToastService toastService)
    {
        _api = api;
        _suppliersApi = suppliersApi;
        _ingredientsApi = ingredientsApi;
        _toastService = toastService;
        _dialogService = dialogService;

        this.WhenAnyValue(x => x.SelectedItem)
            .Subscribe(async item =>
            {
                Title = item == null
                    ? ConstantMessages.CreateTitlePage
                    : ConstantMessages.EditorTitlePage;

                if (SupplierAutocompleteItems.Count == 0)
                    await LoadAdditionalData();

                if (item != null)
                    await LoadItemAsync(item.SupplierIngredientId);
            });

        this.WhenAnyValue(x => x.SelectedIngredientItem)
            .Subscribe(item =>  
            {
                if (item != null)
                {
                    ItemEditable.IngredientId = item.Id;
                    UpdateFilteredUnits();

                    // Сбрасываем Unit только если текущий не входит в новый список
                    if (!FilteredUnits.Any(u => u.Value == ItemEditable.Unit))
                    {
                        ItemEditable.Unit = item.BaseUnit;
                    }

                    SelectedUnitItem = FilteredUnits.FirstOrDefault(u => u.Value == ItemEditable.Unit);
                }
                else
                {
                    FilteredUnits.Clear();
                    SelectedUnitItem = null;
                }

                UnitComboBoxPlaceholder = item != null
                    ? "Выберите ед. измерения"
                    : "Сначала выберите ингредиент";
                UnitComboBoxIsEnabled = item != null;
            });

        // Пишем выбор в форму
        this.WhenAnyValue(x => x.SelectedUnitItem)
            .Where(x => x != null)
            .Subscribe(item =>
            {
                if (ItemEditable.Unit != item!.Value)
                    ItemEditable.Unit = item.Value!.Value;
            });

        // Синхронизируем ComboBox, НЕ трогая FilteredUnits
        this.WhenAnyValue(x => x.ItemEditable.Unit)
            .Subscribe(unit =>
            {
                if (SelectedUnitItem?.Value != unit)
                    SelectedUnitItem = FilteredUnits.FirstOrDefault(u => u.Value == unit);
            });

        this.WhenAnyValue(x => x.SelectedSupplierItem)
            .Subscribe(item =>
            {
                if (item != null)
                    ItemEditable.SupplierId = item.Id;
            });
    }

    [ReactiveCommand]
    private async Task LoadAdditionalData()
    {
        IsBusy = true;
        try
        {
            var suppliersTask = _suppliersApi.GetAll();
            var ingredientsTask = _ingredientsApi.GetAll();

            await Task.WhenAll(suppliersTask, ingredientsTask);

            var suppliersResponse = suppliersTask.Result;
            var ingredientsResponse = ingredientsTask.Result;

            if (!suppliersResponse.IsSuccessStatusCode || suppliersResponse.Content == null ||
                !ingredientsResponse.IsSuccessStatusCode || ingredientsResponse.Content == null)
            {
                _toastService.ShowError(ConstantMessages.ErrorLoadDataToast);
                return;
            }

            SupplierAutocompleteItems = suppliersResponse.Content.SuppliersList
                .Select(s => new ItemsAutoCompleteBox(s.SupplierId, s.Name))
                .ToList();

            IngredientsAutocompleteItems = ingredientsResponse.Content.IngredientsList
                .Select(s => new IngredientAutoCompleteItem(s.IngredientId, s.Name, s.BaseUnit))
                .ToList();
        }
        catch
        {
            _dialogService.ShowError(ConstantMessages.ExceptionDialog);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [ReactiveCommand]
    public async Task<bool> SaveAsync()
    {
        if (ItemEditable!.HasErrors)
        {
            _toastService.ShowError(ItemEditable.GetFirstError());
            return false;
        }
        else if (HasErrors)
        {
            _toastService.ShowError(GetFirstError());
            return false;
        }

        if (ItemEditableCache is not null && ItemEditableCache.EqualsValues(ItemEditable))
        {
            _toastService.ShowInfo(ConstantMessages.NoChangesToast);
            return false;
        }

        IsBusy = true;
        try
        {
            if (ItemEditable.SupplierIngredientId == 0)
            {
                var response = await _api.Create(ItemEditable.ToDto());
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
                var response = await _api.Update(ItemEditable.SupplierIngredientId, ItemEditable.ToDto());
                if (response.IsSuccessStatusCode)
                {
                    _toastService.ShowSuccess(ConstantMessages.UpdatedToast);
                    return true;
                }
                else
                {
                    var msg = response.Error != null
                        ? GoldenBreadApiClient.GetErrorMessage(response.Error)
                        : null;

                    _toastService.ShowError(msg);
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

    private async Task LoadItemAsync(int id)
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

            ItemEditable = SupplierIngredientForm.FromDto(response.Content);
            ItemEditableCache = ItemEditable.Clone();
            SelectedSupplierItem = SupplierAutocompleteItems
                .FirstOrDefault(s => s.Id == ItemEditable.SupplierId);
            SelectedIngredientItem = IngredientsAutocompleteItems
                .FirstOrDefault(s => s.Id == ItemEditable.IngredientId);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void UpdateFilteredUnits()
    {
        var baseUnit = SelectedIngredientItem?.BaseUnit ?? ItemEditable.Unit;

        FilteredUnits.Clear();

        switch (baseUnit)
        {
            case IngredientUnit.Kg:
            case IngredientUnit.G:
                FilteredUnits.Add(new(IngredientUnit.Kg, "Килограмм"));
                FilteredUnits.Add(new(IngredientUnit.G, "Грамм"));
                break;
            case IngredientUnit.L:
            case IngredientUnit.Ml:
                FilteredUnits.Add(new(IngredientUnit.L, "Литр"));
                FilteredUnits.Add(new(IngredientUnit.Ml, "Миллилитр"));
                break;
            case IngredientUnit.Pcs:
                FilteredUnits.Add(new(IngredientUnit.Pcs, "Штук"));
                break;
        }
    }
}