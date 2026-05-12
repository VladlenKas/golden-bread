using GoldenBread.Desktop.Features.Common;
using GoldenBread.Desktop.Features.Procurement.PurchasePositions.Models;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Helpers;
using GoldenBread.Desktop.UI.Services;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using static GoldenBread.Desktop.Features.Procurement.PurchasePositions.ViewModels.PurchasePositionEditorPageViewModel;

namespace GoldenBread.Desktop.Features.Procurement.PurchasePositions.ViewModels;

public partial class IngredientDeleteDialogViewModel : ViewModelBase, IDialogAware
{
    private readonly IIngredientsApi _api;
    private readonly ToastService _toastService;
    private TaskCompletionSource<bool>? _dialogTcs;
    private Action? _dismissDialog;
    
    [Reactive] private List<IngredientAutoCompleteItem> _ingredients = new();
    [Reactive] private IngredientAutoCompleteItem? _selectedIngredient;
    [Reactive] private int _recipesCount = 0;
    [Reactive] private int _supplierIngredientsCount = 0;

    public Dictionary<IngredientUnit, string> Units => LocalizedIngredientUnits.Units;

    public IngredientDeleteDialogViewModel(
        IIngredientsApi api,
        ToastService toastService,
        List<IngredientListItem> ingredients)
    {
        _api = api;
        _toastService = toastService;

        Ingredients = ingredients
            .Select(i => new IngredientAutoCompleteItem(i.IngredientId, i.Name, i.BaseUnit))
            .ToList();

        this.WhenAnyValue(x => x.SelectedIngredient)
            .Subscribe(item =>
            {
                if (item == null)
                {
                    RecipesCount = 0;
                    SupplierIngredientsCount = 0;
                    return;
                }

                var original = ingredients.FirstOrDefault(i => i.IngredientId == item.Id);
                if (original != null)
                {
                    RecipesCount = original.RecipesCount;
                    SupplierIngredientsCount = original.SupplierIngredientsCount;
                }
            });
    }

    public void SetDialogCompletionSource(TaskCompletionSource<bool> tcs) => _dialogTcs = tcs;
    public void SetDismissAction(Action dismiss) => _dismissDialog = dismiss;

    [ReactiveCommand]
    private async Task DeleteAsync()
    {
        if (SelectedIngredient == null)
        {
            _toastService.ShowError(ConstantMessages.IngredientRequiredValidation);
            return;
        }
        else if (RecipesCount > 0 || SupplierIngredientsCount > 0)
        {
            var message = string.Format(
                ConstantMessages.IngredientCannotBeDeleted,
                RecipesCount,
                SupplierIngredientsCount);

            _toastService.ShowWarning(message);
            return;
        }

        var response = await _api.Delete(SelectedIngredient.Id);
        if (response.IsSuccessStatusCode)
        {
            _dismissDialog?.Invoke();       
            _dialogTcs?.TrySetResult(true);
        }
    }

    [ReactiveCommand]
    private void Cancel()
    {
        _dismissDialog?.Invoke();
        _dialogTcs?.TrySetResult(false);
    }
}