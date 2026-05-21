using Avalonia.Platform.Storage;
using DynamicData;
using GoldenBread.Desktop.Features.Common;
using GoldenBread.Desktop.Features.Production.OrdersList.Dtos;
using GoldenBread.Desktop.Features.Production.OrdersList.Models;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.Infrastructure.Helpers;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Refit;
using SukiUI.Controls;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace GoldenBread.Desktop.Features.Production.OrdersList.ViewModels;

public partial class OrdersListPageViewModel : PageViewModel, ISukiStackPageTitleProvider
{
    private readonly IOrdersApi _api;
    private readonly DialogService _dialogService;
    private readonly ToastService _toastService;
    private readonly IDocumentsApi _documentsApi;
    private readonly WindowService _windowService;

    [Reactive] private bool _isBusy;
    [Reactive] private string _searchText = "";
    [Reactive] private int _allItemsCount = 0;
    [Reactive] private int _selectedSortIndex;
    [Reactive] private DateTime? _dateFrom;
    [Reactive] private DateTime? _dateTo;
    [Reactive] private bool _canDragged = true;
    [Reactive] public KanbanItem? _selectedItem;

    public string Title { get; set; } = "Заказы производства";

    private readonly SourceList<KanbanItem> _createdItems = new();
    private readonly SourceList<KanbanItem> _inProgressItems = new();
    private readonly SourceList<KanbanItem> _completedItems = new();
    private readonly SourceList<KanbanItem> _canceledItems = new();

    public ReadOnlyObservableCollection<KanbanItem> CreatedItems { get; }
    public ReadOnlyObservableCollection<KanbanItem> InProgressItems { get; }
    public ReadOnlyObservableCollection<KanbanItem> CompletedItems { get; }
    public ReadOnlyObservableCollection<KanbanItem> CanceledItems { get; }

    public static readonly Dictionary<string, string> ColumnNames = new()
    {
        ["Created"] = "Необработанные",
        ["InProgress"] = "В процессе",
        ["Completed"] = "Готовые",
        ["Canceled"] = "Отмененные"
    };

    public OrdersListPageViewModel(
        IOrdersApi api,
        IDocumentsApi documentsApi,
        WindowService windowService,
        DialogService dialogService,
        ToastService toastService)
    {
        _api = api;
        _documentsApi = documentsApi;
        _windowService = windowService;
        _dialogService = dialogService;
        _toastService = toastService;

        var searchFilter = this.WhenAnyValue(
            x => x.SearchText,
            x => x.SelectedSortIndex,
            x => x.DateFrom,
            x => x.DateTo)
            .Select(_ => SearchPredicate);

        var sortComparer = this.WhenAnyValue(x => x.SelectedSortIndex)
            .Select(_ => SortExpression)
            .DistinctUntilChanged();

        _createdItems.Connect()
            .Filter(searchFilter)
            .Sort(sortComparer)
            .Bind(out var c)
            .Subscribe(_ => UpdateTotalCountItems());

        _inProgressItems.Connect()
            .Filter(searchFilter)
            .Sort(sortComparer)
            .Bind(out var p)
            .Subscribe(_ => UpdateTotalCountItems());

        _completedItems.Connect()
            .Filter(searchFilter)
            .Sort(sortComparer)
            .Bind(out var comp)
            .Subscribe(_ => UpdateTotalCountItems());

        _canceledItems.Connect()
            .Filter(searchFilter)
            .Sort(sortComparer)
            .Bind(out var can)
            .Subscribe(_ => UpdateTotalCountItems());

        CreatedItems = c;
        InProgressItems = p;
        CompletedItems = comp;
        CanceledItems = can;

        // Автозагрузка
        RefreshAsync().ConfigureAwait(false);
    }

    private Func<KanbanItem, bool> SearchPredicate => item =>
    {
        // Текстовый поиск
        var textMatch = string.IsNullOrWhiteSpace(SearchText) ||
            item.SearchText.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase);

        // Фильтр по дате создания
        var dateMatch = true;
        if (DateFrom.HasValue)
            dateMatch = dateMatch && item.EndDate >= DateOnly.FromDateTime(DateFrom.Value);
        if (DateTo.HasValue)
            dateMatch = dateMatch && item.EndDate <= DateOnly.FromDateTime(DateTo.Value);

        return textMatch && dateMatch;
    };

    private IComparer<KanbanItem> SortExpression
    {
        get
        {
            var (field, asc) = SelectedSortIndex switch
            {
                0 => ("id", true),
                1 => ("id", false),
                2 => ("created", true),
                3 => ("created", false),
                4 => ("end", true),
                5 => ("end", false),
                6 => ("amount", true),
                7 => ("amount", false),
                8 => ("items", true),
                9 => ("items", false),
                _ => ("id", true)
            };

            return field switch
            {
                "id" => asc
                    ? Comparer<KanbanItem>.Create((a, b) => a.Id.CompareTo(b.Id))
                    : Comparer<KanbanItem>.Create((a, b) => b.Id.CompareTo(a.Id)),
                "created" => asc
                    ? Comparer<KanbanItem>.Create((a, b) => (a.CreatedAt ?? DateTime.MinValue).CompareTo(b.CreatedAt ?? DateTime.MinValue))
                    : Comparer<KanbanItem>.Create((a, b) => (b.CreatedAt ?? DateTime.MinValue).CompareTo(a.CreatedAt ?? DateTime.MinValue)),
                "end" => asc
                    ? Comparer<KanbanItem>.Create((a, b) => a.EndDate.CompareTo(b.EndDate))
                    : Comparer<KanbanItem>.Create((a, b) => b.EndDate.CompareTo(a.EndDate)),
                "amount" => asc
                    ? Comparer<KanbanItem>.Create((a, b) => a.TotalAmount.CompareTo(b.TotalAmount))
                    : Comparer<KanbanItem>.Create((a, b) => b.TotalAmount.CompareTo(a.TotalAmount)),
                "items" => asc
                    ? Comparer<KanbanItem>.Create((a, b) => a.TotalOrderItems.CompareTo(b.TotalOrderItems))
                    : Comparer<KanbanItem>.Create((a, b) => b.TotalOrderItems.CompareTo(a.TotalOrderItems)),
                _ => Comparer<KanbanItem>.Create((a, b) => a.Id.CompareTo(b.Id))
            };
        }
    }

    private int UpdateTotalCountItems() =>
        AllItemsCount = CreatedItems.Count + InProgressItems.Count + CompletedItems.Count + CanceledItems.Count;

    [ReactiveCommand]
    private void ResetFilters()
    {
        SearchText = string.Empty;
        SelectedSortIndex = 0;
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
                    Id = dto.OrderId,
                    Title = $"Заказ №{dto.OrderId}",
                    Description = dto.CompanyName,
                    Status = dto.Status.ToString(),
                    CompanyName = dto.CompanyName,
                    CreatedAt = dto.CreatedAt,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    TotalAmount = dto.TotalAmount,
                    TotalOrderItems = dto.TotalOrderItems,
                    CompletedOrderItems = dto.CompletedOrderItems
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
        _createdItems.Clear();
        _inProgressItems.Clear();
        _completedItems.Clear();
        _canceledItems.Clear();
    }

    public async Task<bool> ConfirmMoveAsync(string fromColumn, string toColumn, KanbanItem item)
    {
        CanDragged = false;

        var message = $"Переместить «{item.Title}» из «{ColumnNames[fromColumn]}» в «{ColumnNames[toColumn]}»?";
        var tcs = _toastService.ShowWarningQuestion(message);
        var confirmed = await tcs.Task;

        CanDragged = true;
        return confirmed;
    }

    public async void MoveItem(string fromColumn, string toColumn, KanbanItem item)
    {
        var allowed = (fromColumn, toColumn) switch
        {
            ("Created", "InProgress") => true,
            ("Created", "Completed") => true,
            ("Created", "Canceled") => true,
            ("InProgress", "Completed") => true,
            ("InProgress", "Canceled") => true,
            _ => false
        };

        if (!allowed)
        {
            _toastService.ShowWarning("Перемещение в выбранную колонку запрещено");
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
    private async Task<bool> UpdateStatusAsync(
        KanbanItem item, 
        string newStatus, 
        string fromColumn = "", 
        string toColumn = "")
    {
        if (!Enum.TryParse<OrderStatus>(newStatus, out var status))
            return false;

        try
        {
            var request = new UpdateOrderStatusRequest(item.Id, status, null);
            var response = await _api.UpdateStatus(request);

            if (!response.IsSuccessStatusCode)
            {
                var msg = response.Error != null
                    ? GoldenBreadApiClient.GetErrorMessage(response.Error)
                    : null;

                _toastService.ShowErrorImportant(msg);
                return false;
            }
            else
            {
                _toastService.ShowSuccess($"Карточка «{item.Title}» перемещена из «{ColumnNames[fromColumn]}» в «{ColumnNames[toColumn]}»");
                return true;
            }
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
            await RefreshAsync();
        }
    }

    [ReactiveCommand]
    private void Add() { }

    [ReactiveCommand]
    private async Task ShowDetailAsync(KanbanItem? item)
    {
        if (item == null) return;
        var detail = await _api.GetById(item.Id);
        if (!detail.IsSuccessStatusCode || detail.Content == null) return;

        var vm = DetailDialogFactory.FromOrder(detail.Content);
        _dialogService.ShowDetailViewModel(vm);
    }

    [ReactiveCommand]
    private async Task DownloadDocumentAsync(KanbanItem? item)
    {
        if (item is null) return;

        try
        {
            IsBusy = true;

            // 1. Скачиваем с сервера
            var response = await _documentsApi.DownloadDeliveryInvoiceAsync(item.Id);

            if (!response.IsSuccessStatusCode || response.Content is null)
            {
                _toastService.ShowError("Не удалось скачать документ");
                return;
            }

            // 2. Диалог сохранения
            var window = _windowService.GetMenuWindow();
            if (window is null)
            {
                _toastService.ShowError("Окно не найдено");
                return;
            }

            var file = await window.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Сохранить накладную",
                SuggestedFileName = $"Накладная_№{item.Id}.xlsx",
                DefaultExtension = "xlsx",
                FileTypeChoices = new[]
                {
                new FilePickerFileType("Excel (*.xlsx)") { Patterns = new[] { "*.xlsx" } }
            }
            });

            if (file is null)
                return; // пользователь отменил

            // 3. Записываем на диск
            await using (var stream = response.Content)
            await using (var fileStream = await file.OpenWriteAsync())
            {
                await stream.CopyToAsync(fileStream);
            }

            _toastService.ShowSuccess("Документ сохранён");
        }
        catch (ApiException ex)
        {
            _toastService.ShowError(GoldenBreadApiClient.GetErrorMessage(ex));
        }
        catch (Exception ex)
        {
            _toastService.ShowError($"Ошибка сохранения: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private SourceList<KanbanItem>? GetList(string key) => key switch
    {
        "Created" => _createdItems,
        "InProgress" => _inProgressItems,
        "Completed" => _completedItems,
        "Canceled" => _canceledItems,
        _ => null
    };
}