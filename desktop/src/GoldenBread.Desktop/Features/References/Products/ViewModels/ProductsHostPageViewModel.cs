using GoldenBread.Desktop.Features.References.Products.Forms;
using GoldenBread.Desktop.Features.References.Products.Models;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace GoldenBread.Desktop.Features.References.Products.ViewModels;

public partial class ProductsHostPageViewModel : HostPageViewModel
{
    private readonly PageFactory _factory;
    private readonly IProductsApi _api;
    private readonly ToastService _toastService;
    private readonly DialogService _dialogService;

    private readonly ProductsListPageViewModel _listPage;

    // VM для создания — создаём один раз и переиспользуем
    private ProductEditorPageViewModel? _productEditor;
    private ProductRecipeEditorPageViewModel? _recipeEditor;
    private ProductBatchEditorPageViewModel? _batchEditor;
    private ProductImageEditorPageViewModel? _imageEditor;

    // Черновик создания продукта
    private ProductCreationDraft? _draft;

    // Текущие подписки на команды шага (отменяем при переходе на другой шаг)
    private readonly SerialDisposable _saveSubscription = new();
    private readonly SerialDisposable _backSubscription = new();

    public ProductsHostPageViewModel(
        PageFactory factory,
        IProductsApi api,
        ToastService toastService,
        DialogService dialogService)
    {
        _factory = factory;
        _api = api;
        _toastService = toastService;
        _dialogService = dialogService;

        _listPage = factory.GetPage <ProductsListPageViewModel>();

        // Редактирование существующих
        _listPage.EditCommand.Subscribe(item => ShowEditor(item));
        _listPage.EditRecipeCommand.Subscribe(item => ShowRecipeEditor(item));
        _listPage.EditBatchesCommand.Subscribe(item => ShowBatchEditor(item));
        _listPage.EditImagesCommand.Subscribe(item => ShowImageEditor(item));

        // Создание нового
        _listPage.AddCommand.Subscribe(_ => StartCreation());

        // Диалоги категорий
        _listPage.ShowCreateCategoryDialogCommand.Subscribe(_ => { });
        _listPage.ShowEditCategoryDialogCommand.Where(a => a).Subscribe(_ => ShowList());
        _listPage.ShowDeleteCategoryDialogCommand.Where(a => a).Subscribe(_ => ShowList());
    }

    // === СОЗДАНИЕ ПРОДУКТА ===

    private void StartCreation()
    {
        _draft = new ProductCreationDraft();

        _productEditor ??= _factory.GetPage<ProductEditorPageViewModel>();
        _recipeEditor ??= _factory.GetPage<ProductRecipeEditorPageViewModel>();
        _batchEditor ??= _factory.GetPage<ProductBatchEditorPageViewModel>();
        _imageEditor ??= _factory.GetPage<ProductImageEditorPageViewModel>();

        // Сброс в режим создания
        _productEditor.ProductId = 0;
        _recipeEditor.ProductId = 0;
        _batchEditor.ProductId = 0;
        _imageEditor.ProductId = 0;

        // Очистка коллекций от прошлого создания
        _recipeEditor.Recipes.Clear();
        _batchEditor.Batches.Clear();
        _batchEditor.CostPrice = 0;
        _imageEditor.ImagePaths.Clear();

        ShowProductStep();
    }

    private void ShowProductStep()
    {
        ClearStepSubscriptions();

        _saveSubscription.Disposable = _productEditor!.SaveCommand
            .Where(ok => ok)
            .Take(1)
            .Subscribe(_ =>
            {
                _draft!.Product = _productEditor.ItemEditable;
                ShowRecipeStep();
            });

        _backSubscription.Disposable = _productEditor.GoBackCommand
            .Take(1)
            .Subscribe(_ => CancelCreation());

        NavigateTo(_productEditor);
    }

    private void ShowRecipeStep()
    {
        ClearStepSubscriptions();

        _saveSubscription.Disposable = _recipeEditor!.SaveCommand
            .Where(ok => ok)
            .Take(1)
            .Subscribe(_ =>
            {
                _draft!.Recipes = _recipeEditor.Recipes.ToList();
                ShowBatchStep();
            });

        _backSubscription.Disposable = _recipeEditor.GoBackCommand
            .Take(1)
            .Subscribe(_ => ShowProductStep());

        NavigateTo(_recipeEditor);
    }

    private void ShowBatchStep()
    {
        ClearStepSubscriptions();

        if (_draft?.Product != null)
            _batchEditor!.CostPrice = _draft.Product.CostPrice;

        _saveSubscription.Disposable = _batchEditor!.SaveCommand
            .Where(ok => ok)
            .Take(1)
            .Subscribe(_ =>
            {
                _draft!.Batches = _batchEditor.Batches.ToList();
                ShowImageStep();
            });

        _backSubscription.Disposable = _batchEditor.GoBackCommand
            .Take(1)
            .Subscribe(_ => ShowRecipeStep());

        NavigateTo(_batchEditor);
    }

    private void ShowImageStep()
    {
        ClearStepSubscriptions();

        _saveSubscription.Disposable = _imageEditor!.SaveCommand
            .Where(ok => ok)
            .Take(1)
            .Subscribe(async _ =>
            {
                _draft!.ImagePaths = _imageEditor.ImagePaths.Select(i => i.FileName).ToList();
                await FinishCreationAsync();
            });

        _backSubscription.Disposable = _imageEditor.GoBackCommand
            .Take(1)
            .Subscribe(_ => ShowBatchStep());

        NavigateTo(_imageEditor);
    }

    private void ClearStepSubscriptions()
    {
        _saveSubscription.Disposable = null;
        _backSubscription.Disposable = null;
    }

    private async Task FinishCreationAsync()
    {
        if (_draft == null) return;

        try
        {
            // Убеждаемся, что изображения загружены на сервер (SaveAsync в imageEditor это делает)
            var imagePaths = _imageEditor!.ImagePaths.Select(i => i.FileName).ToList();

            var command = new CreateProductWithDetailsCommand(
                _draft.Product.ToDto(),
                _draft.Recipes.Select(r => r.ToDto()).ToList(),
                _draft.Batches.Select(b => b.ToDto(0)).ToList(),
                imagePaths);

            var response = await _api.Create(command);

            if (response.IsSuccessStatusCode)
            {
                _toastService.ShowSuccess(ConstantMessages.CreatedToast);
                _draft = null;
                ShowList();
            }
            else
            {
                _toastService.ShowError();
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"{ConstantMessages.ExceptionDialog}: {ex.Message}");
        }
    }

    private void CancelCreation()
    {
        ClearStepSubscriptions();
        _draft = null;
        ShowList();
    }

    // === РЕДАКТИРОВАНИЕ СУЩЕСТВУЮЩИХ ===

    private void ShowEditor(ProductListItem? item)
    {
        if (item == null) return;
        var page = _factory.GetPage<ProductEditorPageViewModel>();
        page.ProductId = item.ProductId;

        page.SaveCommand.Where(a => a).Take(1).Subscribe(_ => ShowList());
        page.GoBackCommand.Take(1).Subscribe(_ => ShowList());

        NavigateTo(page);
    }

    private void ShowRecipeEditor(ProductListItem? item)
    {
        if (item == null) return;
        var page = _factory.GetPage<ProductRecipeEditorPageViewModel>();
        page.ProductId = item.ProductId;

        page.SaveCommand.Where(a => a).Take(1).Subscribe(_ => ShowList());
        page.GoBackCommand.Take(1).Subscribe(_ => ShowList());

        NavigateTo(page);
    }

    private void ShowBatchEditor(ProductListItem? item)
    {
        if (item == null) return;
        var page = _factory.GetPage<ProductBatchEditorPageViewModel>();
        page.ProductId = item.ProductId;

        page.SaveCommand.Where(a => a).Take(1).Subscribe(_ => ShowList());
        page.GoBackCommand.Take(1).Subscribe(_ => ShowList());

        NavigateTo(page);
    }

    private void ShowImageEditor(ProductListItem? item)
    {
        if (item == null) return;
        var page = _factory.GetPage<ProductImageEditorPageViewModel>();
        page.ProductId = item.ProductId;

        page.SaveCommand.Where(a => a).Take(1).Subscribe(_ => ShowList());
        page.GoBackCommand.Take(1).Subscribe(_ => ShowList());

        NavigateTo(page);
    }

    // === ОБЩЕЕ ===

    private void ShowList()
    {
        ClearStepSubscriptions();
        _draft = null;
        _listPage.RefreshCommand.Execute();
        NavigateTo(_listPage);
    }

    protected override void OnActivated()
    {
        _listPage.Permissions = this.Permissions;
        ShowList();
    }

    protected override void OnDeactivated() => ShowList();
}

public class ProductCreationDraft
{
    public ProductForm Product { get; set; } = new();
    public List<RecipeItemForm> Recipes { get; set; } = new();
    public List<ProductBatchForm> Batches { get; set; } = new();
    public List<string> ImagePaths { get; set; } = new();
}