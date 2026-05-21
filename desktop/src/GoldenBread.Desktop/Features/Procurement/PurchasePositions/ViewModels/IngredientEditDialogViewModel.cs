using GoldenBread.Desktop.Features.Common;
using GoldenBread.Desktop.Features.Procurement.PurchasePositions.Models;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Helpers;
using GoldenBread.Desktop.UI.Services;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.ComponentModel.DataAnnotations;

namespace GoldenBread.Desktop.Features.Procurement.PurchasePositions.ViewModels;

public partial class IngredientEditDialogViewModel : ViewModelBase, IDialogAware
{
    private readonly IIngredientsApi _api;
    private readonly ToastService _toastService;
    private TaskCompletionSource<bool>? _dialogTcs;
    private Action? _dismissDialog;

    [Reactive] private List<IngredientAutoCompleteItem> _ingredients = new();
    [Reactive] private IngredientAutoCompleteItem? _selectedIngredient;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    [StringLength(100, MinimumLength = 2, ErrorMessage = ConstantMessages.NameLengthValidation)]
    string _name = null!;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    IngredientUnit _baseUnit;

    public Dictionary<IngredientUnit, string> Units => LocalizedIngredientUnits.Units;

    public IngredientEditDialogViewModel(
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
                if (item != null)
                {
                    Name = item.Name;
                    BaseUnit = item.BaseUnit;
                }
            });
    }

    public void SetDialogCompletionSource(TaskCompletionSource<bool> tcs) => _dialogTcs = tcs;
    public void SetDismissAction(Action dismiss) => _dismissDialog = dismiss;

    [ReactiveCommand]
    private async Task SaveAsync()
    {
        if (SelectedIngredient == null)
        {
            _toastService.ShowError(ConstantMessages.IngredientRequiredValidation);
            return;
        }

        if (HasErrors)
        {
            _toastService.ShowError(GetFirstError());
            return;
        }

        var response = await _api.Update(new IngredientDto(SelectedIngredient.Id, Name, BaseUnit));
        if (response.IsSuccessStatusCode)
        {
            _dismissDialog?.Invoke();
            _dialogTcs?.TrySetResult(true);
        }
        else
        {
            var msg = response.Error != null
                        ? GoldenBreadApiClient.GetErrorMessage(response.Error)
                        : null;

            _toastService.ShowError(msg);
        }
    }

    [ReactiveCommand]
    private void Cancel()
    {
        _dismissDialog?.Invoke();
        _dialogTcs?.TrySetResult(false);
    }
}