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
            UpdateCanSelectDate();
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
    }

    private void UpdateCartState()
    {
        TotalPrice = Form.CartItems.Sum(x => x.TotalCost);
        TotalUnits = Form.CartItems.Sum(x => x.TotalUnits);
        HasCartItems = Form.CartItems.Any();
        IsCartEmpty = !HasCartItems;
    }

    private void UpdateCanSelectDate()
    {
        CanSelectDate = Form.CompanyId > 0 && HasCartItems && !IsOrderTooLarge;
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
            CostPrice = product.CostPrice
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
        {
            Form.CartItems.Remove(item);
            UpdateCartState();
        }
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
                ? ApiExtensions.GetErrorMessage(response.Error)
                : "Не удалось создать заказ";

            _toastService.ShowError(msg);
            return false;
        }
        catch (ApiException ex)
        {
            _toastService.ShowError(ApiExtensions.GetErrorMessage(ex));
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