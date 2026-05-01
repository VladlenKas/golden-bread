using GoldenBread.Desktop.Features.References.Employees.Models;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using SukiUI.Controls;

namespace GoldenBread.Desktop.Features.References.Employees.ViewModels;

public partial class EditableEmployeePageViewModel : PageViewModel, ISukiStackPageTitleProvider
{
    private readonly IEmployeesApi _api;
    private readonly ToastService _toastService;
    private readonly DialogService _dialogService;

    [Reactive] private EmployeeResponse? _itemResponse;
    [Reactive] private EmployeeListItem? _selectedItem;
    [Reactive] private bool _isBusy;

    private EmployeeResponse? ItemResponseCache { get; set; }
    public string Title { get; set; } = "Добавление";

    public EditableEmployeePageViewModel(
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
                    ItemResponse = new(); 
                    ItemResponseCache = null; 
                    return;
                }

                await LoadEmployeeAsync(item.EmployeeId);
            });
    }

    private async Task LoadEmployeeAsync(int id)
    {
        IsBusy = true;

        try
        {
            var response = await _api.GetById(id);

            if (!response.IsSuccessStatusCode || response.Content == null)
            {
                _dialogService.ShowError(ConstantMessages.ErrorException);
                return;
            }

            ItemResponse = response.Content;
            ItemResponseCache = ItemResponse.Clone();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [ReactiveCommand]
    public async Task SaveAsync()
    {
        if (ItemResponse!.HasErrors)
        {
            _toastService.ShowError(ItemResponse!.GetFirstError() ?? "Ошибка");
        }
        else if (ItemResponseCache != null && ItemResponseCache.EqualsValues(ItemResponse))
        {
            _toastService.ShowInfo("Вы не внесли изменений");
        }
        else
        {
            _toastService.ShowSuccess("Ура");
        }
    }

    [ReactiveCommand]
    public async Task GoBackAsync()
    {

    }
}
