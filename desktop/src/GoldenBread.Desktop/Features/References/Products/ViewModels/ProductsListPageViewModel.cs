using Avalonia.Media.Imaging;
using Avalonia.Threading;
using DynamicData;
using GoldenBread.Desktop.Configuration.Files;
using GoldenBread.Desktop.Features.Procurement.PurchasePositions.ViewModels;
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

public partial class ProductsListPageViewModel : PageViewModel, ISukiStackPageTitleProvider
{
    private readonly IProductsApi _api;
    private readonly IIngredientsApi _ingredientsApi;
    private readonly HttpClient _httpClient;
    private readonly IProductCategoriesApi _categoriesApi;
    private readonly DialogService _dialogService;
    private readonly ToastService _toastService;
    private readonly SourceList<ProductListItem> _sourceList = new();

    [Reactive] private bool _isBusy;
    [Reactive] public bool _isEmpty = true;
    [Reactive] private string _searchText = string.Empty;
    [Reactive] public ProductListItem? _selectedItem;
    [Reactive] private string _selectedBaseFilter = "Все";
    [Reactive] private SeasonFilterOption? _selectedSeasonFilter;

    public List<string> BaseFilters { get; } = new()
    {
        "Все",
        "Самая покупаемая",
        "Самый дорогой",
        "Самый дешевый",
        "По алфавиту А-Я",
        "По алфавиту Я-А",
        "Быстрее готовится",
        "Дольше готовится",
        "Сначала новые",
        "Сначала старые",
    };

    public List<SeasonFilterOption> SeasonFilters { get; }
    public string Title { get; set; } = "Список продукции";
    public ReadOnlyObservableCollection<ProductListItem> FilteredItems { get; }

    public ProductsListPageViewModel(
        IIngredientsApi ingredientsApi,
        IProductsApi api,
        GoldenBreadApiClient apiClient,
        IProductCategoriesApi categoriesApi,
        DialogService dialogService,
        ToastService toastService)
    {
        _api = api;
        _ingredientsApi = ingredientsApi;
        _httpClient = apiClient.HttpClient;
        _categoriesApi = categoriesApi;
        _dialogService = dialogService;
        _toastService = toastService;

        SeasonFilters = GenerateSeasonFilters();

        var textFilter = this.WhenAnyValue(x => x.SearchText)
            .DistinctUntilChanged()
            .Select(SearchFilter);

        var sortComparer = this.WhenAnyValue(
                x => x.SelectedBaseFilter,
                x => x.SelectedSeasonFilter)
            .DistinctUntilChanged()
            .Select(t => BuildComparer(t.Item1, t.Item2));

        _sourceList.Connect()
            .Filter(textFilter)
            .Sort(sortComparer)
            .Bind(out var filtered)
            .Subscribe(_ => IsEmpty = filtered.Count == 0);

        FilteredItems = filtered;
    }

    private static IComparer<ProductListItem> BuildComparer(string baseFilter, SeasonFilterOption? season)
    {
        switch (baseFilter)
        {
            case "Самая покупаемая":
                if (season != null)
                {
                    return Comparer<ProductListItem>.Create((a, b) =>
                    {
                        var aSold = a.SeasonalSales
                            .FirstOrDefault(s => s.Season == season.Season && s.Year == season.Year)
                            ?.TotalUnitsSold ?? 0;
                        var bSold = b.SeasonalSales
                            .FirstOrDefault(s => s.Season == season.Season && s.Year == season.Year)
                            ?.TotalUnitsSold ?? 0;
                        return bSold.CompareTo(aSold);
                    });
                }
                return Comparer<ProductListItem>.Create((a, b) =>
                    b.TotalSoldAllTime.CompareTo(a.TotalSoldAllTime));

            case "Самый дорогой":
                return Comparer<ProductListItem>.Create((a, b) =>
                    b.SalePrice.CompareTo(a.SalePrice));

            case "Самый дешевый":
                return Comparer<ProductListItem>.Create((a, b) =>
                    a.SalePrice.CompareTo(b.SalePrice));

            case "По алфавиту А-Я":
                return Comparer<ProductListItem>.Create((a, b) =>
                    string.Compare(a.Name, b.Name, StringComparison.InvariantCultureIgnoreCase));

            case "По алфавиту Я-А":
                return Comparer<ProductListItem>.Create((a, b) =>
                    string.Compare(b.Name, a.Name, StringComparison.InvariantCultureIgnoreCase));

            case "Быстрее готовится":
                return Comparer<ProductListItem>.Create((a, b) =>
                    a.ProductionTimeMinutes.CompareTo(b.ProductionTimeMinutes));

            case "Дольше готовится":
                return Comparer<ProductListItem>.Create((a, b) =>
                    b.ProductionTimeMinutes.CompareTo(a.ProductionTimeMinutes));
            case "Сначала новые":
                return Comparer<ProductListItem>.Create((a, b) => b.CreatedAt.CompareTo(a.CreatedAt));
            case "Сначала старые":
                return Comparer<ProductListItem>.Create((a, b) => a.CreatedAt.CompareTo(b.CreatedAt));

            default: // "Все"
                return Comparer<ProductListItem>.Create((a, b) =>
                    string.Compare(a.Name, b.Name, StringComparison.InvariantCultureIgnoreCase));
        }
    }

    private static List<SeasonFilterOption> GenerateSeasonFilters()
    {
        var now = DateOnly.FromDateTime(DateTime.UtcNow);
        var list = new List<SeasonFilterOption > ();
        for (int i = 0; i < 5; i++)
        {
            var d = now.AddMonths(-i * 3);
            list.Add(new SeasonFilterOption(d.GetSeason(), d.Year));
        }
        list.Reverse();
        return list;
    }

    private static Func<ProductListItem, bool> SearchFilter(string? search)
    {
        if (string.IsNullOrWhiteSpace(search)) return _ => true;
        var lower = search.ToLowerInvariant();
        return item => item.SearchText.Contains(lower, StringComparison.InvariantCultureIgnoreCase);
    }

    [ReactiveCommand]
    private async Task RefreshAsync()
    {
        try
        {
            IsBusy = true;
            var response = await _api.GetAll();
            if (!response.IsSuccessStatusCode || response.Content == null) return;

            // ★ ОТЛАДКА
            var firstWithSales = response.Content.ProductsList.FirstOrDefault(x => x.TotalSoldAllTime > 0);
            System.Diagnostics.Debug.WriteLine(
                $"Products: {response.Content.ProductsList.Count}, " +
                $"First with sales: {firstWithSales?.Name ?? "NONE"}, " +
                $"Sold: {firstWithSales?.TotalSoldAllTime ?? 0}");

            _sourceList.Clear();
            foreach (var item in response.Content.ProductsList)
            {
                _sourceList.Add(item);
                _ = LoadProductImageAsync(item);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Refresh error: {ex}");
            _dialogService.ShowInfo(ConstantMessages.ExceptionDialog);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [ReactiveCommand]
    private async Task DeleteAsync(ProductListItem? item)
    {
        if (item == null)
        {
            _toastService.ShowError(ConstantMessages.EmptySelectedItem);
            return;
        }

        var tcs = _dialogService.ShowWarningQuestion(ConstantMessages.ProductDeleteConfirmDialog);
        bool confirmed = await tcs.Task;
        if (!confirmed) return;

        IsBusy = true;
        try
        {
            var response = await _api.Delete(item.ProductId);
            if (response.IsSuccessStatusCode)
            {
                _sourceList.Remove(item);
                _toastService.ShowSuccess(ConstantMessages.DeletedToast);
            }
            else _toastService.ShowError();
        }
        catch { _dialogService.ShowError(); }
        finally { IsBusy = false; }
    }

    [ReactiveCommand]
    private async Task ShowDetailAsync(ProductListItem? item)
    {
        if (item == null) return;
        var detail = await _api.GetDetail(item.ProductId);
        if (!detail.IsSuccessStatusCode || detail.Content == null) return;

        var vm = DetailDialogFactory.FromProduct(detail.Content);
        _dialogService.ShowDetailViewModel(vm);
    }


    [ReactiveCommand]
    private async Task ShowCreateIngredientDialog()
    {
        var vm = new IngredientCreateDialogViewModel(_ingredientsApi, _toastService);
        var tcs = _dialogService.ShowDialogAsync(vm, "Новый ингредиент");

        bool confirmed = await tcs.Task;
        if (!confirmed) return;

        _toastService.ShowSuccess(ConstantMessages.CreatedToast);
    }

    [ReactiveCommand]
    private async Task<bool> ShowEditIngredientDialog()
    {
        var ingredients = await _ingredientsApi.GetAll();
        if (!ingredients.IsSuccessStatusCode) return false;

        var vm = new IngredientEditDialogViewModel(
            _ingredientsApi,
            _toastService,
            ingredients.Content!.IngredientsList);

        var tcs = _dialogService.ShowDialogAsync(vm, "Новый ингредиент");

        bool confirmed = await tcs.Task;
        if (!confirmed) return false;

        _toastService.ShowSuccess(ConstantMessages.UpdatedToast);
        return true;
    }

    [ReactiveCommand]
    private async Task<bool> ShowDeleteIngredientDialog()
    {
        var ingredients = await _ingredientsApi.GetAll();
        if (!ingredients.IsSuccessStatusCode) return false;

        var vm = new IngredientDeleteDialogViewModel(
            _ingredientsApi,
            _toastService,
            ingredients.Content!.IngredientsList);

        var tcs = _dialogService.ShowDialogAsync(vm, "Удалить ингредиент");

        bool confirmed = await tcs.Task;
        if (!confirmed) return false;

        _toastService.ShowSuccess(ConstantMessages.DeletedToast);
        return true;
    }

    [ReactiveCommand]
    private async Task<bool> ShowEditCategoryDialog()
    {
        var categories = await _categoriesApi.GetAll();
        if (!categories.IsSuccessStatusCode) return false;

        var vm = new ProductCategoryEditDialogViewModel(
            _categoriesApi, _toastService, categories.Content!.CategoriesList);

        var tcs = _dialogService.ShowDialogAsync(vm, "Изменить категорию");

        bool confirmed = await tcs.Task;
        if (!confirmed) return false;

        _toastService.ShowSuccess(ConstantMessages.UpdatedToast);
        return true;
    }

    [ReactiveCommand]
    private async Task<bool> ShowDeleteCategoryDialog()
    {
        var categories = await _categoriesApi.GetAll();
        if (!categories.IsSuccessStatusCode) return false;

        var vm = new ProductCategoryDeleteDialogViewModel(
            _categoriesApi, _toastService, categories.Content!.CategoriesList);

        var tcs = _dialogService.ShowDialogAsync(vm, "Удалить категорию");

        bool confirmed = await tcs.Task;
        if (!confirmed) return false;

        _toastService.ShowSuccess(ConstantMessages.DeletedToast);
        return true;
    }

    [ReactiveCommand]
    private async Task<bool> ShowCreateCategoryDialog()
    {
        var vm = new ProductCategoryCreateDialogViewModel(_categoriesApi, _toastService);
        var tcs = _dialogService.ShowDialogAsync(vm, "Новая категория");

        bool confirmed = await tcs.Task;
        if (!confirmed) return false;

        _toastService.ShowSuccess(ConstantMessages.CreatedToast);
        return true;
    }

    [ReactiveCommand]
    private async Task<ProductListItem?> EditRecipeAsync(ProductListItem? item) => item;

    [ReactiveCommand]
    private async Task<ProductListItem?> EditBatchesAsync(ProductListItem? item) => item;

    [ReactiveCommand]
    private async Task<ProductListItem?> EditImagesAsync(ProductListItem? item) => item;

    [ReactiveCommand] private async Task AddAsync() { }
    [ReactiveCommand] private async Task<ProductListItem?> EditAsync(ProductListItem? item) => item;

    private async Task LoadProductImageAsync(ProductListItem item)
    {
        if (string.IsNullOrWhiteSpace(item.ImageUrl)) return;

        try
        {
            var url = $"{AppSettings.ApiDbUploadUrl}/{item.ImageUrl}";
            var bytes = await _httpClient.GetByteArrayAsync(url);

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                var ms = new MemoryStream(bytes);
                item.ProductImage = new Bitmap(ms);
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Image load failed for {item.Name}: {ex.Message}");
        }
    }
}