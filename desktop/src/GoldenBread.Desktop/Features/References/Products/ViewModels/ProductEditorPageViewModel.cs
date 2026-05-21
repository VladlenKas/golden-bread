using GoldenBread.Desktop.Features.References.Products.Forms;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using SukiUI.Controls;
using System.ComponentModel.DataAnnotations;
using System.Reactive.Linq;

namespace GoldenBread.Desktop.Features.References.Products.ViewModels;

public partial class ProductEditorPageViewModel : PageViewModel, ISukiStackPageTitleProvider
{
    private readonly IProductsApi _api;
    private readonly IProductCategoriesApi _categoriesApi;
    private readonly ToastService _toastService;
    private readonly DialogService _dialogService;

    [Reactive] private bool _isBusy;
    [Reactive] private ProductForm _itemEditable = new();
    [Reactive] public int _productId;
    [Reactive] private List<ItemsAutoCompleteBox> _categoryAutocompleteItems = new ();

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.CategoryRequiredValidation)]
    ItemsAutoCompleteBox? _selectedCategoryItem;

    private ProductForm? ItemEditableCache { get; set; }
    public string Title { get; set; } = ConstantMessages.EditorTitlePage;
    public string SaveButtonContent { get; set; } = "Сохранить";

    public ProductEditorPageViewModel(
        IProductsApi api,
        IProductCategoriesApi categoriesApi,
        DialogService dialogService,
        ToastService toastService)
    {
        _api = api;
        _categoriesApi = categoriesApi;
        _toastService = toastService;
        _dialogService = dialogService;

        this.WhenAnyValue(x => x.ProductId)
            .Subscribe(async id =>
            {
                Title = id > 0
                    ? ConstantMessages.EditorTitlePage
                    : ConstantMessages.CreateTitlePage;

                SaveButtonContent = id > 0
                    ? "Сохранить"
                    : "Далее";

                if (CategoryAutocompleteItems.Count == 0)
                    await LoadAdditionalData();

                if (id > 0)
                    await LoadProductAsync(id);
            });

        this.WhenAnyValue(x => x.SelectedCategoryItem)
            .Subscribe(item =>
            {
                if (item != null)
                    ItemEditable.CategoryId = item.Id;
            });
    }

    public void InitializeDraft(ProductForm form)
    {
        ItemEditable = form.Clone();          // или new ProductForm { ... } если нет Clone
        ItemEditableCache = ItemEditable.Clone(); // чтобы сравнение "нет изменений" работало
    }

    [ReactiveCommand]
    private async Task LoadAdditionalData()
    {
        IsBusy = true;
        try
        {
            var response = await _categoriesApi.GetAll();
            if (response.IsSuccessStatusCode && response.Content != null)
            {
                CategoryAutocompleteItems = response.Content.CategoriesList
                    .Select(c => new ItemsAutoCompleteBox(c.ProductCategoryId, c.Name))
                    .ToList();
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

    private async Task LoadProductAsync(int id)
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

            ItemEditable = ProductForm.FromDto(response.Content);
            ItemEditableCache = ItemEditable.Clone();
            SelectedCategoryItem = CategoryAutocompleteItems
                .FirstOrDefault(c => c.Id == ItemEditable.CategoryId);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [ReactiveCommand]
    public async Task<bool> SaveAsync()
    {
        if (ItemEditable.HasErrors)
        {
            _toastService.ShowError(ItemEditable.GetFirstError());
            return false;
        }
        else if (HasErrors)
        {
            _toastService.ShowError(GetFirstError());
            return false;
        }

        if (ProductId == 0)
            return true;

        if (ItemEditableCache != null && ItemEditableCache.EqualsValues(ItemEditable))
        {
            _toastService.ShowInfo(ConstantMessages.NoChangesToast);
            return false;
        }

        IsBusy = true;
        try
        {
            var response = await _api.Update(ItemEditable.ToDto());
            if (response.IsSuccessStatusCode)
            {
                _toastService.ShowSuccess(ConstantMessages.UpdatedToast);
                return true;
            }

            var msg = response.Error != null
                ? GoldenBreadApiClient.GetErrorMessage(response.Error)
                : null;

            _toastService.ShowError(msg);
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