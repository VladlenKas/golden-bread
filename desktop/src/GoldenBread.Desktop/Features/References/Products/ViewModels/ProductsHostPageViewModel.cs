using GoldenBread.Desktop.Features.References.Products.Forms;
using GoldenBread.Desktop.Features.References.Products.Models;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;

namespace GoldenBread.Desktop.Features.References.Products.ViewModels;

public partial class ProductsHostPageViewModel : HostPageViewModel
{
    private readonly PageFactory _factory;
    private readonly IProductsApi _api;
    private readonly ToastService _toastService;
    private readonly DialogService _dialogService;

    private readonly ProductsListPageViewModel _listPage;

    // === СТЕК СОЗДАНИЯ ===
    // Храним только пройденные VM. Текущая — последняя в списке.
    // При "Далее" от конкретной VM обрезаем всё, что было после неё, и пушим новую.
    // При "Назад" от конкретной VM удаляем её и всё после, показываем предыдущую.
    private readonly List<PageViewModel> _creationStack = new ();
    private readonly Dictionary<PageViewModel, CompositeDisposable> _stepDisposables = new ();

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
        ClearCreationStack();

        var editor = _factory.GetPage<ProductEditorPageViewModel>();
        editor.ProductId = 0;

        PushStep(editor);
    }

    // --- Переходы вперёд ---

    private void ShowRecipeStep(ProductEditorPageViewModel from)
    {
        var editor = _factory.GetPage<ProductRecipeEditorPageViewModel>();
        editor.ProductId = 0;
        PushAfter(from, editor);
    }

    private void ShowBatchStep(ProductRecipeEditorPageViewModel from)
    {
        var editor = _factory.GetPage<ProductBatchEditorPageViewModel>();
        editor.ProductId = 0;

        // CostPrice берём из данных продукта, который уже в стеке
        var productEditor = _creationStack.OfType<ProductEditorPageViewModel>().FirstOrDefault();
        if (productEditor?.ItemEditable != null)
            editor.CostPrice = productEditor.ItemEditable.CostPrice;

        PushAfter(from, editor);
    }

    private void ShowImageStep(ProductBatchEditorPageViewModel from)
    {
        var editor = _factory.GetPage<ProductImageEditorPageViewModel>();
        editor.ProductId = 0;
        PushAfter(from, editor);
    }

    // --- Управление стеком ---

    /// <summary>
    /// Добавляет первую страницу в чистый стек.
    /// </summary>
    private void PushStep(PageViewModel page)
    {
        _creationStack.Add(page);
        SubscribeToStep(page);
        NavigateTo(page);
    }

    /// <summary>
    /// Вызывается при нажатии "Далее" на странице <paramref name="from"/>.
    /// Обрезает всё, что лежало впереди (пользователь мог перескочить назад),
    /// и создаёт новую страницу после <paramref name="from"/>.
    /// </summary>
    private void PushAfter(PageViewModel from, PageViewModel newPage)
    {
        var index = _creationStack.IndexOf(from);
        if (index >= 0 && index < _creationStack.Count - 1)
        {
            // Удаляем "мёртвые" страницы, которые SukiUI уже выкинул,
            // но мы всё ещё держим в памяти
            for (int i = _creationStack.Count - 1; i > index; i--)
            {
                RemoveStepAt(i);
            }
        }

        _creationStack.Add(newPage);
        SubscribeToStep(newPage);
        NavigateTo(newPage);
    }

    /// <summary>
    /// Вызывается при нажатии "Назад" на странице <paramref name="from"/>.
    /// Удаляет её и всё после (хотя после обычно ничего нет),
    /// показывает предыдущую страницу из стека.
    /// </summary>
    private void GoBackFrom(PageViewModel from)
    {
        var index = _creationStack.IndexOf(from);
        if (index < 0) return; // не нашли — игнорируем

        if (index == 0)
        {
            // На первом шаге — отмена всего создания
            ClearCreationStack();
            ShowList();
            return;
        }

        // Удаляем текущую и всё, что теоретически могло быть после
        for (int i = _creationStack.Count - 1; i >= index; i--)
        {
            RemoveStepAt(i);
        }

        // Показываем предыдущую, которая теперь последняя
        var prev = _creationStack.LastOrDefault();
        if (prev != null)
            NavigateTo(prev);
        else
            ShowList();
    }

    private void RemoveStepAt(int index)
    {
        var page = _creationStack[index];
        if (_stepDisposables.Remove(page, out var d))
            d.Dispose();

        _creationStack.RemoveAt(index);
    }

    private void ClearCreationStack()
    {
        foreach (var d in _stepDisposables.Values)
            d.Dispose();

        _stepDisposables.Clear();
        _creationStack.Clear();
    }

    // --- Подписки на команды шагов (один раз на жизнь VM) ---

    private void SubscribeToStep(PageViewModel page)
    {
        // Если уже подписаны — не дублируем
        if (_stepDisposables.ContainsKey(page)) return;

        var d = new CompositeDisposable();

        switch (page)
        {
            case ProductEditorPageViewModel editor:
                editor.SaveCommand
                    .Where(ok => ok)
                    .Subscribe(_ => ShowRecipeStep(editor))
                    .DisposeWith(d);

                editor.GoBackCommand
                    .Subscribe(_ => GoBackFrom(editor))
                    .DisposeWith(d);
                break;

            case ProductRecipeEditorPageViewModel editor:
                editor.SaveCommand
                    .Where(ok => ok)
                    .Subscribe(_ => ShowBatchStep(editor))
                    .DisposeWith(d);

                editor.GoBackCommand
                    .Subscribe(_ => GoBackFrom(editor))
                    .DisposeWith(d);
                break;

            case ProductBatchEditorPageViewModel editor:
                editor.SaveCommand
                    .Where(ok => ok)
                    .Subscribe(_ => ShowImageStep(editor))
                    .DisposeWith(d);

                editor.GoBackCommand
                    .Subscribe(_ => GoBackFrom(editor))
                    .DisposeWith(d);
                break;

            case ProductImageEditorPageViewModel editor:
                editor.SaveCommand
                    .Where(ok => ok)
                    .Subscribe(async _ => await FinishCreationAsync())
                    .DisposeWith(d);

                editor.GoBackCommand
                    .Subscribe(_ => GoBackFrom(editor))
                    .DisposeWith(d);
                break;
        }

        _stepDisposables[page] = d;
    }

    // --- Финализация ---

    private async Task FinishCreationAsync()
    {
        var productEditor = _creationStack.OfType<ProductEditorPageViewModel>().FirstOrDefault();
        var recipeEditor = _creationStack.OfType<ProductRecipeEditorPageViewModel>().FirstOrDefault();
        var batchEditor = _creationStack.OfType<ProductBatchEditorPageViewModel>().FirstOrDefault();
        var imageEditor = _creationStack.OfType<ProductImageEditorPageViewModel>().FirstOrDefault();

        if (productEditor?.ItemEditable == null)
        {
            _toastService.ShowError("Не заполнены основные данные продукта");
            return;
        }

        try
        {
            var command = new CreateProductWithDetailsCommand(
                productEditor.ItemEditable.ToDto(),
                recipeEditor?.Recipes.Select(r => r.ToDto()).ToList() ?? new(),
                batchEditor?.Batches.Select(b => b.ToDto(0)).ToList() ?? new(),
                imageEditor?.ImagePaths.Select(i => i.FileName).ToList() ?? new());

            var response = await _api.Create(command);

            if (response.IsSuccessStatusCode)
            {
                _toastService.ShowSuccess(ConstantMessages.CreatedToast);
                ClearCreationStack();
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

    // === РЕДАКТИРОВАНИЕ СУЩЕСТВУЮЩИХ (одноразовые страницы, вне стека создания) ===

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
        ClearCreationStack();
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