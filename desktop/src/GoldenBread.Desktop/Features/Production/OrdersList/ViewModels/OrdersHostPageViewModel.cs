using DynamicData;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;
using SukiUI.Controls;
using System.Collections.ObjectModel;
using ReactiveUI.SourceGenerators;
using System.Reactive.Linq;

namespace GoldenBread.Desktop.Features.Production.OrdersList.ViewModels;

public partial class OrdersHostPageViewModel : HostPageViewModel, ISukiStackPageTitleProvider
{
    private readonly ToastService _toastService;
    private readonly DialogService _dialogService;

    [Reactive] private bool _isBusy;
    public string Title { get; set; } = "Заказы производства";

    // --- 4 колонки через SourceList (реактивно) ---
    private readonly SourceList<KanbanItem> _newItems = new();
    private readonly SourceList<KanbanItem> _inProgressItems = new();
    private readonly SourceList<KanbanItem> _reviewItems = new();
    private readonly SourceList<KanbanItem> _archiveItems = new();

    public ReadOnlyObservableCollection<KanbanItem> NewItems { get; }
    public ReadOnlyObservableCollection<KanbanItem> InProgressItems { get; }
    public ReadOnlyObservableCollection<KanbanItem> ReviewItems { get; }
    public ReadOnlyObservableCollection<KanbanItem> ArchiveItems { get; }

    private static readonly Dictionary<string, string> ColumnNames = new()
    {
        ["New"] = "Новые",
        ["InProgress"] = "В работе",
        ["Review"] = "На проверке",
        ["Archive"] = "Архив"
    };

    public OrdersHostPageViewModel(ToastService toastService, DialogService dialogService)
    {
        _toastService = toastService;
        _dialogService = dialogService;

        _newItems.Connect().Bind(out var n).Subscribe();
        _inProgressItems.Connect().Bind(out var p).Subscribe();
        _reviewItems.Connect().Bind(out var r).Subscribe();
        _archiveItems.Connect().Bind(out var a).Subscribe();

        NewItems = n;
        InProgressItems = p;
        ReviewItems = r;
        ArchiveItems = a;

        LoadTestData();
    }

    /// <summary>
    /// Показывает диалог подтверждения перед перемещением.
    /// </summary>
    public async Task<bool> ConfirmMoveAsync(string fromColumn, string toColumn, KanbanItem item)
    {
        var message = $"Переместить «{item.Title}» из «{ColumnNames[fromColumn]}» в «{ColumnNames[toColumn]}»?";
        var tcs = _dialogService.ShowWarningQuestion(message);
        return await tcs.Task;
    }

    /// <summary>
    /// Перемещает карточку между колонками. Вызывается из Drop в code-behind.
    /// </summary>
    public void MoveItem(string fromColumn, string toColumn, KanbanItem item)
    {
        if (toColumn == "Archive")
        {
            _toastService.ShowWarning("В архив нельзя перемещать карточки напрямую");
            return;
        }

        var from = GetList(fromColumn);
        var to = GetList(toColumn);

        if (from == null || to == null)
            return;

        if (!from.Items.Contains(item))
            return;

        from.Remove(item);
        item.Status = toColumn;
        to.Add(item);

        _toastService.ShowSuccess(
            $"Карточка «{item.Title}» перемещена из «{ColumnNames[fromColumn]}» в «{ColumnNames[toColumn]}»");
    }

    private SourceList<KanbanItem>? GetList(string key) => key switch
    {
        "New" => _newItems,
        "InProgress" => _inProgressItems,
        "Review" => _reviewItems,
        "Archive" => _archiveItems,
        _ => null
    };

    private void LoadTestData()
    {
        _newItems.Add(new KanbanItem { Id = 1, Title = "Заказ #101", Description = "Багет классический 250г", Status = "New" });
        _newItems.Add(new KanbanItem { Id = 2, Title = "Заказ #102", Description = "Круассан с миндалем", Status = "New" });
        _newItems.Add(new KanbanItem { Id = 3, Title = "Заказ #103", Description = "Чиабатта итальянская", Status = "New" });

        _inProgressItems.Add(new KanbanItem { Id = 4, Title = "Заказ #97", Description = "Хлеб ржаной завтрашний", Status = "InProgress" });
        _inProgressItems.Add(new KanbanItem { Id = 5, Title = "Заказ #98", Description = "Булочки с корицей", Status = "InProgress" });

        _reviewItems.Add(new KanbanItem { Id = 6, Title = "Заказ #95", Description = "Пирог с яблоком 1кг", Status = "Review" });

        _archiveItems.Add(new KanbanItem { Id = 7, Title = "Заказ #90", Description = "Сдоба вишневая", Status = "Archive" });
        _archiveItems.Add(new KanbanItem { Id = 8, Title = "Заказ #91", Description = "Батон нарезной", Status = "Archive" });
    }
}

// Модель карточки (можно вынести в отдельный файл при желании)
public class KanbanItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}