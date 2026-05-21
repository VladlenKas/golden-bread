using Avalonia.Media.Imaging;
using Avalonia.Threading;
using DynamicData;
using GoldenBread.Desktop.Configuration.Files;
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

public partial class ProductsListPageViewModel : PageViewModel, ISukiStackPageTitleProvider
{
    private readonly IProductsApi _api;
    private readonly HttpClient _httpClient;
    private readonly IProductCategoriesApi _categoriesApi;
    private readonly DialogService _dialogService;
    private readonly ToastService _toastService;
    private readonly SourceList<ProductListItem> _sourceList = new();

    [Reactive] private bool _isBusy;
    [Reactive] public bool _isEmpty = true;
    [Reactive] private string _searchText = string.Empty;
    [Reactive] public ProductListItem? _selectedItem;

    public string Title { get; set; } = ConstantMessages.HostTitlePage;
    public ReadOnlyObservableCollection<ProductListItem> FilteredItems { get; }

    public ProductsListPageViewModel(
        IProductsApi api,
        GoldenBreadApiClient apiClient,
        IProductCategoriesApi categoriesApi,
        DialogService dialogService,
        ToastService toastService)
    {
        _api = api;
        _httpClient = apiClient.HttpClient;
        _categoriesApi = categoriesApi;
        _dialogService = dialogService;
        _toastService = toastService;

        var filter = this.WhenAnyValue(x => x.SearchText)
            .DistinctUntilChanged()
            .Select(SearchFilter);

        _sourceList.Connect()
            .Filter(filter)
            .Bind(out var filtered)
            .Subscribe(_ => IsEmpty = filtered.Count == 0);

        FilteredItems = filtered;
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

            _sourceList.Clear();
            foreach (var item in response.Content.ProductsList)
            {
                _sourceList.Add(item);
                // Загружаем картинку в фоне, не блокируя UI
                _ = LoadProductImageAsync(item);
            }
        }
        catch
        {
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
    private async Task<bool> ShowEditCategoryDialog()
    {
        var categories = await _categoriesApi.GetAll();
        if (!categories.IsSuccessStatusCode) return false;

        var vm = new ProductCategoryEditDialogViewModel(
            _categoriesApi, _toastService, categories.Content!.CategoriesList);

        var tcs = _dialogService.ShowDialogAsync(vm, "Изменить категорию");
        return await tcs.Task;
    }

    [ReactiveCommand]
    private async Task<bool> ShowDeleteCategoryDialog()
    {
        var categories = await _categoriesApi.GetAll();
        if (!categories.IsSuccessStatusCode) return false;

        var vm = new ProductCategoryDeleteDialogViewModel(
            _categoriesApi, _toastService, categories.Content!.CategoriesList);

        var tcs = _dialogService.ShowDialogAsync(vm, "Удалить категорию");
        return await tcs.Task;
    }

    [ReactiveCommand]
    private async Task<bool> ShowCreateCategoryDialog()
    {
        var vm = new ProductCategoryCreateDialogViewModel(_categoriesApi, _toastService);
        var tcs = _dialogService.ShowDialogAsync(vm, "Новая категория");
        return await tcs.Task;
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