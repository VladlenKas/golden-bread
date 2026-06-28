using DynamicData;
using GoldenBread.Desktop.Features.Production.EmployeeTasksList.Dtos;
using GoldenBread.Desktop.Features.Production.EmployeeTasksList.Models;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Refit;
using SukiUI.Controls;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GoldenBread.Desktop.Features.Production.EmployeeTasksList.ViewModels;

public partial class EmployeeTasksListPageViewModel : PageViewModel, ISukiStackPageTitleProvider
{
    private readonly IEmployeeTasksApi _api;
    private readonly DialogService _dialogService;
    private readonly ToastService _toastService;

    [Reactive] private bool _isBusy;
    [Reactive] private string _searchText = "";
    [Reactive] private int _allItemsCount = 0;
    [Reactive] private int _selectedSortIndex;
    [Reactive] private bool _canDragged = true;
    [Reactive] public KanbanItem? _selectedItem;
    [Reactive] private int _selectedDateFilterIndex;
    [Reactive] private DateTime? _dateFrom;
    [Reactive] private DateTime? _dateTo;

    public string Title { get; set; } = "Задачи сотрудников";

    private readonly SourceList<KanbanItem> _inProgressItems = new();
    private readonly SourceList<KanbanItem> _pausedItems = new();
    private readonly SourceList<KanbanItem> _completedItems = new();
    private readonly SourceList<KanbanItem> _canceledItems = new();

    public ReadOnlyObservableCollection<KanbanItem> InProgressItems { get; }
    public ReadOnlyObservableCollection<KanbanItem> PausedItems { get; }
    public ReadOnlyObservableCollection<KanbanItem> CompletedItems { get; }
    public ReadOnlyObservableCollection<KanbanItem> CanceledItems { get; }

    public static readonly Dictionary<string, string> ColumnNames = new()
    {
        ["InProgress"] = "В работе",
        ["Paused"] = "Приостановлены",
        ["Completed"] = "Выполнены",
        ["Canceled"] = "Отменены"
    };

    public EmployeeTasksListPageViewModel(
        IEmployeeTasksApi api,
        DialogService dialogService,
        ToastService toastService)
    {
        _api = api;
        _dialogService = dialogService;
        _toastService = toastService;

        var searchFilter = this.WhenAnyValue(
            x => x.SearchText,
            x => x.SelectedSortIndex,
            x => x.SelectedDateFilterIndex,
            x => x.DateFrom,
            x => x.DateTo)
            .Select(_ => SearchPredicate);

        var sortComparer = this.WhenAnyValue(x => x.SelectedSortIndex)
            .Select(_ => SortExpression)
            .DistinctUntilChanged();

        _inProgressItems.Connect()
            .Filter(searchFilter)
            .Sort(sortComparer)
            .Bind(out var p)
            .Subscribe(_ => UpdateTotalCountItems());

        _pausedItems.Connect()
            .Filter(searchFilter)
            .Sort(sortComparer)
            .Bind(out var pa)
            .Subscribe(_ => UpdateTotalCountItems());

        _completedItems.Connect()
            .Filter(searchFilter)
            .Sort(sortComparer)
            .Bind(out var c)
            .Subscribe(_ => UpdateTotalCountItems());

        _canceledItems.Connect()
            .Filter(searchFilter)
            .Sort(sortComparer)
            .Bind(out var ca)
            .Subscribe(_ => UpdateTotalCountItems());

        InProgressItems = p;
        PausedItems = pa;
        CompletedItems = c;
        CanceledItems = ca;

        RefreshAsync().ConfigureAwait(false);
    }

    private Func<KanbanItem, bool> SearchPredicate => item =>
    {
        var textMatch = string.IsNullOrWhiteSpace(SearchText) ||
            item.SearchText.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase);

        // Фильтр по дате начала задачи
        var dateMatch = true;
        if (DateFrom.HasValue && item.StartTime.HasValue)
            dateMatch = dateMatch && DateOnly.FromDateTime(item.StartTime.Value.DateTime) >= DateOnly.FromDateTime(DateFrom.Value);
        if (DateTo.HasValue && item.StartTime.HasValue)
            dateMatch = dateMatch && DateOnly.FromDateTime(item.StartTime.Value.DateTime) <= DateOnly.FromDateTime(DateTo.Value);

        // Быстрый фильтр (как раньше)
        var quickFilterMatch = SelectedDateFilterIndex switch
        {
            0 => true,
            1 => item.StartTime.HasValue && item.StartTime.Value.Date == DateTimeOffset.Now.Date,
            2 => item.StartTime.HasValue && item.StartTime.Value.Date >= DateTimeOffset.Now.Date,
            3 => item.StartTime.HasValue && item.StartTime.Value.Date < DateTimeOffset.Now.Date
                 && item.Status is not "Completed" and not "Canceled",
            4 => !item.StartTime.HasValue,
            _ => true
        };

        return textMatch && dateMatch && quickFilterMatch;
    };

    private IComparer<KanbanItem> SortExpression
    {
        get
        {
            var (field, asc) = SelectedSortIndex switch
            {
                0 => ("id", true),
                1 => ("id", false),
                2 => ("employee", true),
                3 => ("employee", false),
                4 => ("start", true),
                5 => ("start", false),
                6 => ("progress", true),
                7 => ("progress", false),
                _ => ("id", true)
            };

            return field switch
            {
                "id" => asc
                    ? Comparer<KanbanItem>.Create((a, b) => a.Id.CompareTo(b.Id))
                    : Comparer<KanbanItem>.Create((a, b) => b.Id.CompareTo(a.Id)),
                "employee" => asc
                    ? Comparer<KanbanItem>.Create((a, b) =>
                        string.Compare(a.EmployeeName, b.EmployeeName, StringComparison.InvariantCultureIgnoreCase))
                    : Comparer<KanbanItem>.Create((a, b) =>
                        string.Compare(b.EmployeeName, a.EmployeeName, StringComparison.InvariantCultureIgnoreCase)),
                "start" => asc
                    ? Comparer<KanbanItem>.Create((a, b) =>
                        (a.StartTime ?? DateTimeOffset.MinValue).CompareTo(b.StartTime ?? DateTimeOffset.MinValue))
                    : Comparer<KanbanItem>.Create((a, b) =>
                        (b.StartTime ?? DateTimeOffset.MinValue).CompareTo(a.StartTime ?? DateTimeOffset.MinValue)),
                "progress" => asc
                    ? Comparer<KanbanItem>.Create((a, b) => a.Progress.CompareTo(b.Progress))
                    : Comparer<KanbanItem>.Create((a, b) => b.Progress.CompareTo(a.Progress)),
                _ => Comparer<KanbanItem>.Create((a, b) => a.Id.CompareTo(b.Id))
            };
        }
    }

    private int UpdateTotalCountItems() =>
        AllItemsCount = InProgressItems.Count + PausedItems.Count
                      + CompletedItems.Count + CanceledItems.Count;

    [ReactiveCommand]
    private void ResetFilters()
    {
        SearchText = string.Empty;
        SelectedSortIndex = 0;
        SelectedDateFilterIndex = 0;
        DateFrom = null;
        DateTo = null;
    }

    [ReactiveCommand]
    private async Task RefreshAsync()
    {
        try
        {
            IsBusy = true;
            var response = await _api.GetKanban();
            if (!response.IsSuccessStatusCode || response.Content == null)
            {
                _toastService.ShowError(ConstantMessages.ErrorLoadDataToast);
                return;
            }

            ClearColumns();
            foreach (var dto in response.Content)
            {
                var card = new KanbanItem
                {
                    Id = dto.EmployeeTaskId,
                    Title = $"Задача №{dto.EmployeeTaskId}",
                    Description = dto.EmployeeName,
                    Status = dto.Status.ToString(),
                    EmployeeName = dto.EmployeeName,
                    ProductName = dto.ProductName,
                    OrderId = dto.OrderId,
                    CompanyName = dto.CompanyName,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    AssignedQuantity = dto.AssignedQuantity,
                    CompletedQuantity = dto.CompletedQuantity
                };
                GetList(card.Status)?.Add(card);
            }
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

    private void ClearColumns()
    {
        _inProgressItems.Clear();
        _pausedItems.Clear();
        _completedItems.Clear();
        _canceledItems.Clear();
    }

    public async void MoveItem(string fromColumn, string toColumn, KanbanItem item)
    {
        if (fromColumn == "Canceled" || toColumn == "Canceled")
        {
            _toastService.ShowWarning("Отмененные задачи нельзя перемещать");
            return;
        }

        if (!item.IsEditable)
        {
            _toastService.ShowWarning("Редактирование недоступно для прошлых задач");
            return;
        }

        bool result = await UpdateStatusAsync(item, toColumn, fromColumn, toColumn);
        if (result)
        {
            var from = GetList(fromColumn);
            var to = GetList(toColumn);
            if (from == null || to == null || !from.Items.Contains(item))
                return;

            from.Remove(item);
            item.Status = toColumn;
            to.Add(item);
        }
    }

    [ReactiveCommand]
    private async Task IncrementProgressAsync(KanbanItem? item)
    {
        if (item == null) return;
        if (!item.IsEditable)
        {
            _toastService.ShowWarning("Редактирование недоступно для прошлых задач");
            return;
        }
        if (item.CompletedQuantity >= item.AssignedQuantity) return;

        await UpdateProgressAsync(item, item.CompletedQuantity + 1);
    }

    [ReactiveCommand]
    private async Task DecrementProgressAsync(KanbanItem? item)
    {
        if (item == null) return;
        if (!item.IsEditable)
        {
            _toastService.ShowWarning("Редактирование недоступно для прошлых задач");
            return;
        }
        if (item.CompletedQuantity <= 0) return;

        await UpdateProgressAsync(item, item.CompletedQuantity - 1);
    }

    [ReactiveCommand]
    private async Task<bool> UpdateStatusAsync(
    KanbanItem item,
    string newStatus,
    string fromColumn = "",
    string toColumn = "")
    {
        if (!Enum.TryParse<Features.Common.TaskStatus>(newStatus, out var status))
    {
            _toastService.ShowError($"Не удалось распознать статус '{newStatus}'");
            return false;
        }

        try
        {
            var request = new UpdateEmployeeTaskStatusRequest(item.Id, status);
            var response = await _api.UpdateStatus(request);

            if (!response.IsSuccessStatusCode)
            {
                var msg = response.Error != null
                    ? GoldenBreadApiClient.GetErrorMessage(response.Error)
                    : null;

                _toastService.ShowErrorImportant(msg);
                return false;
            }

            _toastService.ShowSuccess(
                $"Задача «{item.Title}» перемещена из «{ColumnNames[fromColumn]}» в «{ColumnNames[toColumn]}»");
            return true;
        }
        catch (ApiException ex)
        {
            _toastService.ShowError(GoldenBreadApiClient.GetErrorMessage(ex));
            return false;
        }
        catch
        {
            _toastService.ShowError("Ошибка при обновлении статуса");
            return false;
        }
        finally
        {
            // Обновляем данные с сервера, чтобы быть в синхроне
            await RefreshAsync();
        }
    }

    [ReactiveCommand]
    private async Task ShowDetailAsync(KanbanItem? item)
    {
        if (item == null) return;

        var detail = await _api.GetById(item.Id);
        if (!detail.IsSuccessStatusCode || detail.Content == null) return;

        var vm = DetailDialogFactory.FromEmployeeTask(detail.Content);
        _dialogService.ShowDetailViewModel(vm);
    }

    private async Task UpdateProgressAsync(KanbanItem item, int newQuantity)
    {
        try
        {
            var request = new UpdateEmployeeTaskProgressRequest(item.Id, newQuantity);
            var response = await _api.UpdateProgress(request);

            if (!response.IsSuccessStatusCode)
            {
                var msg = response.Error != null
                    ? GoldenBreadApiClient.GetErrorMessage(response.Error)
                    : null;

                _toastService.ShowErrorImportant(msg);
                return;
            }

            item.CompletedQuantity = newQuantity;
            _toastService.ShowSuccess($"Прогресс: {newQuantity}/{item.AssignedQuantity}");
        }
        catch (ApiException ex)
        {
            _toastService.ShowError(GoldenBreadApiClient.GetErrorMessage(ex));
        }
        catch
        {
            _toastService.ShowError("Ошибка при обновлении прогресса");
        }
    }

    private SourceList<KanbanItem>? GetList(string key) => key switch
    {
        "InProgress" => _inProgressItems,
        "Paused" => _pausedItems,
        "Completed" => _completedItems,
        "Canceled" => _canceledItems,
        _ => null
    };
}