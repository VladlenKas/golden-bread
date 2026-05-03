using GoldenBread.Desktop.Features.References.Employees.Models;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using SukiUI.Controls;
using System.Diagnostics;

namespace GoldenBread.Desktop.Features.References.Employees.ViewModels;

public partial class EmployeeEditorPageViewModel : PageViewModel, ISukiStackPageTitleProvider
{
    private readonly IEmployeesApi _api;
    private readonly ToastService _toastService;
    private readonly DialogService _dialogService;

    [Reactive] private EmployeeForm? _itemResponse;
    [Reactive] private EmployeeListItem? _selectedItem;
    [Reactive] private bool _isBusy;

    private EmployeeForm? ItemResponseCache { get; set; }
    public string Title { get; set; } = ConstantMessages.CreateTitlePage;

    public EmployeeEditorPageViewModel(
        IEmployeesApi api,
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
                    ItemResponse = new EmployeeForm();
                    ItemResponseCache = null;

                    return;
                }

                Title = ConstantMessages.EditorTitlePage;
                await LoadEmployeeAsync(item.EmployeeId);
            });
    }

    [ReactiveCommand]
    public async Task<bool> SaveAsync()
    {
        if (ItemResponse!.HasErrors)
        {
            _toastService.ShowError(ItemResponse.GetFirstError());
            return false;
        }

        if (ItemResponseCache is not null && 
            ItemResponseCache.EqualsValues(ItemResponse))
        {
            _toastService.ShowInfo(ConstantMessages.NoChangesToast);
            return false;
        }

        IsBusy = true;
        try
        {
            // Создание
            if (ItemResponse.EmployeeId == 0)
            {
                var command = ItemResponse.ToDto();
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
            // Обновление
            else
            {
                var command = ItemResponse.ToDto();
                var response = await _api.Update(command);

                if (response.IsSuccessStatusCode)
                {
                    _toastService.ShowSuccess(ConstantMessages.UpdatedToast);
                    return true;
                }
                else
                {
                    Debug.WriteLine(response.Error);
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

    [ReactiveCommand] // Оповещение оркестартора
    public async Task GoBackAsync() { }

    private async Task LoadEmployeeAsync(int id)
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

            ItemResponse = EmployeeForm.FromDto(response.Content);
            ItemResponseCache = ItemResponse.Clone();
        }
        finally
        {
            IsBusy = false;
        }
    }
}