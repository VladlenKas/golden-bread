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

namespace GoldenBread.Desktop.Features.References.Employees.ViewModels;

public partial class EmployeesListPageViewModel(
    IEmployeesApi api,
    DialogService dialogService,
    ToastService toastService) : PageViewModel, ISukiStackPageTitleProvider
{
    [Reactive] private bool _isBusy;
    [Reactive] public EmployeeListItem? _selectedItem;
    [Reactive] public ObservableCollection<EmployeeListItem> _itemsList = new();

    public string Title { get; set; } = "Список сотрудников";

    [ReactiveCommand]
    private async Task RefreshAsync()
    {
        try
        {
            IsBusy = true;

            var response = await api.GetAll();
            if (!response.IsSuccessStatusCode || response.Content == null)
                return;

            var data = response.Content;

            ItemsList.Clear();
            foreach (var item in data.EmployeesList)
                ItemsList.Add(item);
        }
        catch (Exception)
        {
            dialogService.ShowInfo(ConstantMessages.ErrorException);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [ReactiveCommand]
    private async Task AddAsync() { }

    [ReactiveCommand]
    private async Task<EmployeeListItem?> EditAsync(EmployeeListItem? selectedItem) => selectedItem;

    // Временная остановка назначения заказов сотруднику
    [ReactiveCommand]
    private async Task PauseAsync()
    {
        toastService.ShowInfo(SelectedItem?.Fullname + " Приостановить");
    }

    [ReactiveCommand]
    private async Task DeleteAsync()
    {
        dialogService.ShowInfo(SelectedItem?.Fullname + " Удалить");
    }
}