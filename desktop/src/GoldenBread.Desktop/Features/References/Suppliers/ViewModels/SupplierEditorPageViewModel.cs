using GoldenBread.Desktop.Features.References.Suppliers.Models;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using SukiUI.Controls;
using ReactiveUI.SourceGenerators;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.UI.Services;
using ReactiveUI;

namespace GoldenBread.Desktop.Features.References.Suppliers.ViewModels;

public partial class SupplierEditorPageViewModel : PageViewModel, ISukiStackPageTitleProvider
{
    private readonly ISuppliersApi _api;
    private readonly ToastService _toastService;
    private readonly DialogService _dialogService;

    [Reactive] private SupplierForm? _ItemEditable;
    [Reactive] private SupplierListItem? _selectedItem;
    [Reactive] private bool _isBusy;

    private SupplierForm? ItemEditableCache { get; set; }
    public string Title { get; set; } = ConstantMessages.CreateTitlePage;

    public SupplierEditorPageViewModel(
        ISuppliersApi api,
        DialogService dialogService,
        ToastService toastService)
    {
        _api = api;
        _toastService = toastService;
        _dialogService = dialogService;

        this.WhenAnyValue(x => x.SelectedItem)
            .Subscribe(async item =>
            {
                if (item == null)
                {
                    ItemEditable = new SupplierForm();
                    ItemEditableCache = null;
                    return;
                }

                Title = ConstantMessages.EditorTitlePage;
                await LoadSupplierAsync(item.SupplierId);
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
            if (ItemEditable.SupplierId == 0)
            {
                var command = ItemEditable.ToDto();
                var response = await _api.Create(command);

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
                var command = ItemEditable.ToDto();
                var response = await _api.Update(command);

                if (response.IsSuccessStatusCode)
                {
                    _toastService.ShowSuccess(ConstantMessages.UpdatedToast);
                    return true;
                }
                else
                {
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

    private async Task LoadSupplierAsync(int id)
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

            ItemEditable = SupplierForm.FromDto(response.Content);
            ItemEditableCache = ItemEditable.Clone();
        }
        finally
        {
            IsBusy = false;
        }
    }
}
