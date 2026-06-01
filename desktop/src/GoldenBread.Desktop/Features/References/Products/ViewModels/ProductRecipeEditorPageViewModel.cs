using GoldenBread.Desktop.Features.References.Products.Forms;
using GoldenBread.Desktop.Features.References.Products.Models;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Helpers;
using GoldenBread.Desktop.UI.Services;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using SukiUI.Controls;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace GoldenBread.Desktop.Features.References.Products.ViewModels;

public partial class ProductRecipeEditorPageViewModel : PageViewModel, ISukiStackPageTitleProvider
{
    private readonly IProductsApi _api;
    private readonly IIngredientsApi _ingredientsApi;
    private readonly ToastService _toastService;
    private readonly DialogService _dialogService;

    [Reactive] private bool _isBusy;
    [Reactive] public bool _isEmpty = true;
    [Reactive] private ObservableCollection<RecipeItemForm> _recipes = new ();
    [Reactive] public int _productId;
    [Reactive] private List<IngredientAutoCompleteItem> _ingredientsAutocompleteItems = new ();
    [Reactive] private IngredientAutoCompleteItem? _selectedIngredientItem;

    private List<RecipeItemForm>? RecipesCache { get; set; }
    public string Title { get; set; } = "Редактирование рецепта";
    public string SaveButtonContent { get; set; } = "Сохранить";

    public ProductRecipeEditorPageViewModel(
        IProductsApi api,
        IIngredientsApi ingredientsApi,
        DialogService dialogService,
        ToastService toastService)
    {
        _api = api;
        _ingredientsApi = ingredientsApi;
        _toastService = toastService;
        _dialogService = dialogService;

        this.WhenAnyValue(x => x.ProductId)
            .Subscribe(async id =>
            {
                Title = id > 0
                    ? "Редактирование рецепта"
                    : "Рецепт продукции";

                SaveButtonContent = id > 0
                    ? "Сохранить"
                    : "Далее";

                if (IngredientsAutocompleteItems.Count == 0)
                    await LoadAdditionalData();

                if (id > 0)
                    await LoadRecipesAsync(id);
            });

        this.WhenAnyValue(x => x.Recipes)
            .Subscribe(items =>
            {
                if (items == null)
                {
                    IsEmpty = true;
                    return;
                }

                IsEmpty = items.Count == 0;

                // Переподписка на новую коллекцию
                items.CollectionChanged += (_, __) => IsEmpty = items.Count == 0;
            });
    }

    [ReactiveCommand]
    private async Task LoadAdditionalData()
    {
        IsBusy = true;
        try
        {
            var response = await _ingredientsApi.GetAll();
            if (response.IsSuccessStatusCode && response.Content != null)
            {
                IngredientsAutocompleteItems = response.Content.IngredientsList
                    .Select(i => new IngredientAutoCompleteItem(i.IngredientId, i.Name, i.BaseUnit))
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

    private async Task LoadRecipesAsync(int id)
    {
        IsBusy = true;
        try
        {
            var response = await _api.GetDetail(id);
            if (!response.IsSuccessStatusCode || response.Content == null)
            {
                _dialogService.ShowError(ConstantMessages.ExceptionDialog);
                return;
            }

            Recipes = new ObservableCollection<RecipeItemForm>(
                response.Content.Ingredients.Select(i => new RecipeItemForm
                {
                    IngredientId = i.IngredientId,
                    Unit = i.Unit,
                    IngredientName = i.Name,
                    Quantity = i.Quantity
                }));

            RecipesCache = Recipes.Select(r => r.Clone()).ToList();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [ReactiveCommand]
    private void AddRecipeItem()
    {
        if (SelectedIngredientItem == null) return;

        // Проверка на дубликат
        if (Recipes.Any(r => r.IngredientId == SelectedIngredientItem.Id))
        {
            _toastService.ShowInfo(ConstantMessages.IngredientExsist);
            return;
        }

        Recipes.Add(new RecipeItemForm
        {
            IngredientId = SelectedIngredientItem.Id,
            IngredientName = SelectedIngredientItem.Name,
            Unit = SelectedIngredientItem.BaseUnit.ToString(),
            Quantity = 0
        });

        SelectedIngredientItem = null;
    }

    [ReactiveCommand]
    private void RemoveRecipeItem(RecipeItemForm? item)
    {
        if (item != null) Recipes.Remove(item);
    }

    [ReactiveCommand]
    public async Task<bool> SaveAsync()
    {
        if (Recipes.Count == 0)
        {
            _toastService.ShowError(ConstantMessages.IngredientsNull);
            return false;
        }

        var recipe = Recipes.FirstOrDefault(r => r.HasErrors);
        if (recipe is not null)
        {
            _toastService.ShowError(recipe.GetFirstError());
            return false;
        }

        // Режим создания: просто подтверждаем шаг
        if (ProductId == 0)
            return true;

        if (RecipesCache != null &&
            !(Recipes.Any(x => !RecipesCache.Any(d => d.EqualsValues(x)))))
        {
            _toastService.ShowInfo(ConstantMessages.NoChangesToast);
            return false;
        }

        IsBusy = true;
        try
        {
            var dtos = Recipes.Select(r => r.ToDto()).ToList();
            var response = await _api.UpdateRecipe(new UpdateProductRecipeCommand(ProductId, dtos));
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

    [ReactiveCommand]
    public async Task GoBackAsync() { }
}