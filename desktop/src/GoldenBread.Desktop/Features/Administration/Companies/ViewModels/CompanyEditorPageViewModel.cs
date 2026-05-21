using GoldenBread.Desktop.Features.Administration.Companies.Models;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using SukiUI.Controls;
using System.Diagnostics;

namespace GoldenBread.Desktop.Features.Administration.Companies.ViewModels;

public partial class CompanyEditorPageViewModel : PageViewModel, ISukiStackPageTitleProvider
{
    private readonly ICompaniesApi _api;
    private readonly ToastService _toastService;
    private readonly DialogService _dialogService;

    [Reactive] private CompanyForm? _itemEditable = new();
    [Reactive] private CompanyListItem? _selectedItem;
    [Reactive] private bool _isBusy;

    private CompanyForm? ItemEditableCache { get; set; }
    public string Title { get; set; } = ConstantMessages.CreateTitlePage;

    public CompanyEditorPageViewModel(
        ICompaniesApi api,
        DialogService dialogService,
        ToastService toastService)
    {
        _api = api;
        _toastService = toastService;
        _dialogService = dialogService;

        this.WhenAnyValue(x => x.SelectedItem)
            .Subscribe(async item =>
            {
                Title = item == null
                  ? ConstantMessages.CreateTitlePage
                  : ConstantMessages.EditorTitlePage;

                if (item != null) await LoadCompanyAsync(item.CompanyId);
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
            if (ItemEditable.CompanyId == 0)
            {
                var request = new CreateCompanyCommand(
                    ItemEditable.ToDto(),
                    ItemEditable.Email!,
                    ItemEditable.Password!);

                var response = await _api.Create(request);

                if (response.IsSuccessStatusCode)
                {
                    _toastService.ShowSuccess(ConstantMessages.CreatedToast);
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
            else
            {
                var dto = ItemEditable.ToDto();
                var response = await _api.Update(dto);

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

    private async Task LoadCompanyAsync(int id)
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

            ItemEditable = CompanyForm.FromDto(response.Content);
            ItemEditableCache = ItemEditable.Clone();
        }
        finally
        {
            IsBusy = false;
        }
    }
}
