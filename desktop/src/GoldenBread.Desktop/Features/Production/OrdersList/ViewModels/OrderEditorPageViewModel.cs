using GoldenBread.Desktop.Features.Production.OrdersList.Dtos;
using GoldenBread.Desktop.Features.Production.OrdersList.Forms;
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
using System.Reactive.Linq;

namespace GoldenBread.Desktop.Features.Production.OrdersList.ViewModels;

public partial class OrderEditorPageViewModel : PageViewModel, ISukiStackPageTitleProvider
{
    private const int MaxProductionTimeMinutes = 8 * 60; // 480 минут

    private readonly IOrdersApi _ordersApi;
    private readonly ToastService _toastService;
    private readonly DialogService _dialogService;

    [Reactive] private bool _isBusy;
    [Reactive] private OrderEditorForm _form = new();
    [Reactive] private List<CompanyLookup> _companies = new ();
    [Reactive] private List<ProductEditorDto> _products = new();
    [Reactive] private List<ItemsAutoCompleteBox> _productAutocompleteItems = new ();
    [Reactive] private CompanyLookup? _selectedCompany;
    [Reactive] private ItemsAutoCompleteBox? _selectedProductAutoCompleteItem;

    [Reactive] private DateTime? _selectedCalendarDate;
    [Reactive] private DateTimeOffset? _minDeliveryDateOffset;

    [Reactive] private decimal _totalPrice;
    [Reactive] private decimal _totalUnits;
    [Reactive] private bool _hasCartItems;
    [Reactive] private bool _isCartEmpty = true;
    [Reactive] private bool _canSelectDate;
    [Reactive] private bool _isOrderTooLarge;

    // Для вычисления времени производства заказа
    [Reactive] private int _totalProductionTimeMinutes;
    [Reactive] private double _productionProgressPercent;
    [Reactive] private bool _isWithinProductionLimit = true;
    [Reactive] private string _formattedProductionTime = "0ч 0мин";

    // Плашки 
    [Reactive] private bool _showSelectCompanyInfo = true;
    [Reactive] private bool _showAddProductsInfo = true;
    [Reactive] private bool _isProductionLimitExceeded;

    public string Title { get; set; } = "Новый заказ";

    public OrderEditorPageViewModel(
        IOrdersApi ordersApi,
        ToastService toastService,
        DialogService dialogService)
    {
        _ordersApi = ordersApi;
        _toastService = toastService;
        _dialogService = dialogService;

        _ = LoadEditorDataAsync();

        // DatePicker <-> DateOnly
        this.WhenAnyValue(x => x.SelectedCalendarDate)
            .Subscribe(dt => Form.SelectedEndDate = dt.HasValue ? DateOnly.FromDateTime(dt.Value.Date) : null);

        this.WhenAnyValue(x => x.Form.MinDeliveryDate)
            .Subscribe(d => MinDeliveryDateOffset = d?.ToDateTime(TimeOnly.MinValue));

        // Компания
        this.WhenAnyValue(x => x.SelectedCompany)
            .Subscribe(c =>
            {
                Form.CompanyId = c?.CompanyId ?? 0;
                UpdateCanSelectDate();
            });

        // Автодобавление продукта при выборе в AutoCompleteBox
        this.WhenAnyValue(x => x.SelectedProductAutoCompleteItem)
            .Where(x => x != null)
            .Subscribe(item =>
            {
                if (item != null)
                {
                    AddProductById(item.Id);
                    SelectedProductAutoCompleteItem = null;
                }
            });

        // Корзина
        Form.CartItems.CollectionChanged += async (_, e) =>
        {
            if (e.NewItems != null)
                foreach (var item in e.NewItems.OfType<OrderCartItem>())
                    ObserveCartItem(item);

            UpdateCartState();      
            await OnCartChangedAsync();
        };
    }

    private void ObserveCartItem(OrderCartItem item)
    {
        item.WhenAnyValue(x => x.TotalCost)
            .Subscribe(_ => UpdateCartState());

        item.WhenAnyValue(x => x.Quantity, x => x.SelectedBatch)
            .Throttle(TimeSpan.FromMilliseconds(100))
            .Subscribe(_ => OnCartChangedAsync().ConfigureAwait(false));

        item.WhenAnyValue(x => x.TotalProductionTimeMinutes)
            .Subscribe(_ => UpdateProductionTime());
    }

    private void UpdateProductionTime()
    {
        TotalProductionTimeMinutes = Form.CartItems.Sum(x => x.TotalProductionTimeMinutes);

        var hours = TotalProductionTimeMinutes / 60;
        var minutes = TotalProductionTimeMinutes % 60;
        FormattedProductionTime = $"{hours}ч {minutes}мин";

        ProductionProgressPercent = Math.Min(
            (double)TotalProductionTimeMinutes / MaxProductionTimeMinutes * 100,
            100);

        IsWithinProductionLimit = TotalProductionTimeMinutes <= MaxProductionTimeMinutes;
        IsProductionLimitExceeded = !IsWithinProductionLimit;

        // ← обновляем CanSelectDate, чтобы блокировать дату при превышении
        UpdateCanSelectDate();
    }

    private void UpdateCartState()
    {
        TotalPrice = Form.CartItems.Sum(x => x.TotalCost);
        TotalUnits = Form.CartItems.Sum(x => x.TotalUnits);
        HasCartItems = Form.CartItems.Any();
        IsCartEmpty = !HasCartItems;

        UpdateProductionTime(); 
    }

    private void UpdateCanSelectDate()
    {
        bool companyOk = Form.CompanyId > 0;
        bool itemsOk = HasCartItems;
        bool sizeOk = !IsOrderTooLarge;
        bool timeOk = IsWithinProductionLimit;

        CanSelectDate = companyOk && itemsOk && sizeOk && timeOk;

        // Показываем "выберите компанию" только если корзина не пуста (иначе в корзине своя плашка),
        // компания не выбрана, и нет проблем с размером/временем
        ShowSelectCompanyInfo = !companyOk && itemsOk && sizeOk && timeOk;
    }

    [ReactiveCommand]
    private async Task LoadEditorDataAsync()
    {
        if (Companies.Any() && Products.Any()) return;

        IsBusy = true;
        try
        {
            var response = await _ordersApi.GetEditorData();
            if (response.IsSuccessStatusCode && response.Content != null)
            {
                Companies = response.Content.Companies;
                Products = response.Content.Products;
                ProductAutocompleteItems = response.Content.Products
                    .Select(p => new ItemsAutoCompleteBox(p.ProductId, p.Name))
                    .ToList();
            }
            else
            {
                _toastService.ShowError("Не удалось загрузить данные для заказа");
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

    private void AddProductById(int productId)
    {
        var product = Products.FirstOrDefault(p => p.ProductId == productId);
        if (product == null) return;

        var usedBatchIds = Form.CartItems
            .Where(c => c.ProductId == productId)
            .Select(c => c.SelectedBatch?.ProductBatchId)
            .ToHashSet();

        var firstAvailableBatch = product.Batches
            .FirstOrDefault(b => !usedBatchIds.Contains(b.ProductBatchId));

        if (firstAvailableBatch == null)
        {
            _toastService.ShowWarning("Все партии этого продукта уже добавлены");
            return;
        }

        var cartItem = new OrderCartItem
        {
            ProductId = product.ProductId,
            ProductName = product.Name,
            CostPrice = product.CostPrice,
            ProductionTimeMinutes = product.ProductionTimeMinutes
        };

        foreach (var b in product.Batches)
        {
            cartItem.AvailableBatches.Add(new ProductBatchOption
            {
                ProductBatchId = b.ProductBatchId,
                MarkupPercent = b.MarkupPercent,
                QuantityUnits = b.QuantityUnits,
                CostPrice = product.CostPrice
            });
        }

        cartItem.SelectedBatch = cartItem.AvailableBatches
            .First(b => b.ProductBatchId == firstAvailableBatch.ProductBatchId);

        Form.CartItems.Add(cartItem);
    }

    [ReactiveCommand]
    private void RemoveProduct(OrderCartItem? item)
    {
        if (item != null)
            Form.CartItems.Remove(item);
    }

    [ReactiveCommand]
    private void IncreaseQuantity(OrderCartItem? item)
    {
        if (item != null) item.Quantity++;
    }

    [ReactiveCommand]
    private void DecreaseQuantity(OrderCartItem? item)
    {
        if (item != null && item.Quantity > 1) item.Quantity--;
    }

    private async Task OnCartChangedAsync()
    {
        if (!Form.CartItems.Any())
        {
            Form.MinDeliveryDate = null;
            Form.MaxDeliveryDate = null;
            Form.SelectedEndDate = null;
            SelectedCalendarDate = null;
            IsOrderTooLarge = false;
            return;
        }

        var drafts = Form.CartItems
            .Select(c => new OrderItemDraft(c.SelectedBatch!.ProductBatchId, c.Quantity))
            .ToList();

        var desired = Form.SelectedEndDate ?? DateOnly.FromDateTime(DateTime.Today.AddMonths(1));
        var request = new CalculateDeliveryRequest(drafts, desired);

        try
        {
            var response = await _ordersApi.CalculateDelivery(request);
            if (response.IsSuccessStatusCode && response.Content != null)
            {
                Form.MinDeliveryDate = response.Content.MinDeliveryDate;
                Form.MaxDeliveryDate = response.Content.MaxDeliveryDate;

                if (Form.MinDeliveryDate >= Form.MaxDeliveryDate)
                {
                    IsOrderTooLarge = true;
                    Form.SelectedEndDate = null;
                    SelectedCalendarDate = null;
                }
                else
                {
                    IsOrderTooLarge = false;

                    // ⬇️ СБРАСЫВАЕМ только если выбранная дата устарела или меньше минимальной
                    if (Form.SelectedEndDate.HasValue && Form.SelectedEndDate < Form.MinDeliveryDate)
                    {
                        Form.SelectedEndDate = null;
                        SelectedCalendarDate = null;
                        _toastService.ShowWarning($"Минимальная дата сдвинулась. Выберите дату не раньше {Form.MinDeliveryDate:d}");
                    }
                    // ⬇️ НЕ ставим автоматом MinDeliveryDate! Пусть пользователь сам выберет.
                }
            }
        }
        catch { }
    }

    [ReactiveCommand]
    public async Task<bool> SaveAsync()
    {
        // ⬇️ ПОЛНАЯ ВАЛИДАЦИЯ ВМЕСТО CanCreate
        if (Form.CompanyId <= 0)
        {
            _toastService.ShowError("Выберите компанию");
            return false;
        }

        if (!HasCartItems)
        {
            _toastService.ShowError("Добавьте продукцию в заказ");
            return false;
        }

        if (IsProductionLimitExceeded)
        {
            _toastService.ShowError($"Превышен лимит 8 часов. Текущее: {FormattedProductionTime}");
            return false;
        }

        if (IsOrderTooLarge)
        {
            _toastService.ShowError("Заказ слишком большой. Уменьшите количество продукции.");
            return false;
        }

        if (!Form.SelectedEndDate.HasValue)
        {
            _toastService.ShowError("Выберите дату доставки");
            return false;
        }

        if (Form.SelectedEndDate < Form.MinDeliveryDate)
        {
            _toastService.ShowError($"Минимальная дата доставки: {Form.MinDeliveryDate:d}");
            return false;
        }

        if (Form.SelectedEndDate > Form.MaxDeliveryDate)
        {
            _toastService.ShowError($"Максимальная дата доставки: {Form.MaxDeliveryDate:d}");
            return false;
        }

        var request = new CreateOrderRequest(
            Form.CompanyId,
            Form.CartItems.Select(c => new OrderItemDraft(c.SelectedBatch!.ProductBatchId, c.Quantity)).ToList(),
            Form.SelectedEndDate!.Value);

        IsBusy = true;
        try
        {
            var response = await _ordersApi.Create(request);
            if (response.IsSuccessStatusCode)
            {
                _toastService.ShowSuccess("Заказ успешно создан");
                return true;
            }

            var msg = response.Error != null
                ? GoldenBreadApiClient.GetErrorMessage(response.Error)
                : null;

            _toastService.ShowError(msg);
            return false;
        }
        catch (ApiException ex)
        {
            _toastService.ShowError(GoldenBreadApiClient.GetErrorMessage(ex));
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

    [ReactiveCommand]
    public async Task GoBackAsync() { }
}