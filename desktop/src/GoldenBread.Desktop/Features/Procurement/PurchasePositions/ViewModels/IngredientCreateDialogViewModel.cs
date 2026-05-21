using GoldenBread.Desktop.Features.Common;
using GoldenBread.Desktop.Features.Procurement.PurchasePositions.Models;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Helpers;
using GoldenBread.Desktop.UI.Services;
using ReactiveUI.SourceGenerators;
using System.ComponentModel.DataAnnotations;

namespace GoldenBread.Desktop.Features.Procurement.PurchasePositions.ViewModels;

public partial class IngredientCreateDialogViewModel(IIngredientsApi api, ToastService toastService) : ViewModelBase, IDialogAware
{
    private TaskCompletionSource<bool>? _dialogTcs;
    private Action? _dismissDialog;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    [StringLength(100, MinimumLength = 2, ErrorMessage = ConstantMessages.NameLengthValidation)]
    string _name = null!;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    IngredientUnit _baseUnit;

    public Dictionary<IngredientUnit, string> Units => LocalizedIngredientUnits.Units;

    public void SetDialogCompletionSource(TaskCompletionSource<bool> tcs) => _dialogTcs = tcs;
    public void SetDismissAction(Action dismiss) => _dismissDialog = dismiss;

    [ReactiveCommand]
    private async Task SaveAsync()
    {
        if (HasErrors)
        {
            toastService.ShowError(GetFirstError());
            return;
        }

        var response = await api.Create(new IngredientDto(0, Name, BaseUnit));
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

            toastService.ShowError(msg);
        }
    }

    [ReactiveCommand]
    private void Cancel()
    {
        _dismissDialog?.Invoke();
        _dialogTcs?.TrySetResult(false);
    }
}