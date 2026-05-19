using GoldenBread.Desktop.Features.References.Products.Forms;
using GoldenBread.Desktop.Features.References.Products.Models;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using SukiUI.Controls;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace GoldenBread.Desktop.Features.References.Products.ViewModels;

#warning Перенести весь текст в константы
public partial class ProductBatchEditorPageViewModel : PageViewModel, ISukiStackPageTitleProvider
{
    private readonly IProductsApi _api;
    private readonly ToastService _toastService;
    private readonly DialogService _dialogService;

    [Reactive] private bool _isBusy;
    [Reactive] public bool _isEmpty = true;
    [Reactive] private ObservableCollection<ProductBatchForm> _batches = new();
    [Reactive] public int _productId;
    [Reactive] public decimal _costPrice;

    private List<ProductBatchForm>? BatchesCache { get; set; }
    public string Title { get; set; } = "Редактирование партий";
    public string SaveButtonContent { get; set; } = "Сохранить";

    public ProductBatchEditorPageViewModel(
        IProductsApi api,
        DialogService dialogService,
        ToastService toastService)
    {
        _api = api;
        _toastService = toastService;
        _dialogService = dialogService;

        this.WhenAnyValue(x => x.ProductId)
            .Subscribe(async id =>
            {
                Title = id > 0
                    ? "Редактирование партий"
                    : "Партии продажи";

                SaveButtonContent = id > 0
                    ? "Сохранить"
                    : "Далее";

                if (id > 0)
                    await LoadAsync(id);
            });

        this.WhenAnyValue(x => x.Batches)
            .Subscribe(items =>
            {
                if (items == null)
                {
                    IsEmpty = true;
                    return;
                }

                IsEmpty = items.Count == 0;
                items.CollectionChanged += (_, __) => IsEmpty = items.Count == 0;
            });
    }

    private async Task LoadAsync(int id)
    {
        IsBusy = true;
        try
        {
            var response = await _api.GetDetail(id);
            if (!response.IsSuccessStatusCode || response.Content == null) return;

            // Базовая цена за 1 шт от продукта
            var basePrice = response.Content.CostPrice;
            CostPrice = basePrice;

            Batches = new ObservableCollection<ProductBatchForm>(
                response.Content.AvailableBatches.Select(b => new ProductBatchForm
                {
                    ProductBatchId = b.ProductBatchId,
                    MarkupPercent = 0,
                    QuantityUnits = b.QuantityPerBatch,
                    BaseUnitPrice = basePrice 
                }));

            IsEmpty = Batches.Count == 0;
            BatchesCache = Batches.Select(b => b.Clone()).ToList();
        }
        finally { IsBusy = false; }
    }

    [ReactiveCommand]
    private void AddBatch()
    {
        if (Batches.Count >= 3)
        {
            _toastService.ShowInfo("Максимум 3 партии");
            return;
        }

        var newBatch = new ProductBatchForm
        {
            MarkupPercent = 0,
            QuantityUnits = 1,
            BaseUnitPrice = CostPrice 
        };

        Batches.Add(newBatch);
        IsEmpty = false;
    }

    [ReactiveCommand]
    private void RemoveBatch(ProductBatchForm? item)
    {
        if (item != null) Batches.Remove(item);
    }

    [ReactiveCommand]
    public async Task<bool> SaveAsync()
    {
        if (Batches.Count == 0)
        {
            _toastService.ShowError("Добавьте хотя бы одну партию");
            return false;
        }

        var batch = Batches.FirstOrDefault(b => b.HasErrors);
        if (batch is not null)
        {
            _toastService.ShowError(batch.GetFirstError());
            return false;
        }

        // Проверка дубликатов перед сохранением
        var hasDuplicates = Batches
            .GroupBy(b => new { b.QuantityUnits, b.MarkupPercent })
            .Any(g => g.Count() > 1);

        if (hasDuplicates)
        {
            _toastService.ShowError("Имеются дублирующиеся партии");
            return false;
        }

        // Режим создания: просто подтверждаем шаг
        if (ProductId == 0)
            return true;

        if (BatchesCache != null && Batches.SequenceEqual(BatchesCache))
        {
            _toastService.ShowInfo(ConstantMessages.NoChangesToast);
            return false;
        }

        IsBusy = true;
        try
        {
            var dtos = Batches.Select(b => b.ToDto(ProductId)).ToList();
            var response = await _api.UpdateBatches(new UpdateProductBatchesCommand(ProductId, dtos));
            if (response.IsSuccessStatusCode)
            {
                _toastService.ShowSuccess(ConstantMessages.UpdatedToast);
                return true;
            }
            _toastService.ShowError();
            return false;
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

    [ReactiveCommand] public async Task GoBackAsync() { }
}