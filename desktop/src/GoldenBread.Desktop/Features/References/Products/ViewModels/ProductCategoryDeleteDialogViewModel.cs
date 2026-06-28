using GoldenBread.Desktop.Features.References.Products.Models;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace GoldenBread.Desktop.Features.References.Products.ViewModels;

public partial class ProductCategoryDeleteDialogViewModel : ViewModelBase, IDialogAware
{
    private readonly IProductCategoriesApi _api;
    private readonly ToastService _toastService;
    private readonly List<ProductCategoryListItem> _originalCategories;

    private TaskCompletionSource<bool>? _tcs;
    private Action? _dismiss;

    [Reactive] private List<ProductCategoryAutoCompleteItem> _categories = new();
    [Reactive] private ProductCategoryAutoCompleteItem? _selectedCategory;
    [Reactive] private int _productsCount;

    public ProductCategoryDeleteDialogViewModel(
        IProductCategoriesApi api,
        ToastService toastService,
        List<ProductCategoryListItem> categories)
    {
        _api = api;
        _toastService = toastService;
        _originalCategories = categories;

        _categories = categories
            .Select(c => new ProductCategoryAutoCompleteItem(c.ProductCategoryId, c.Name, c.Color))
            .ToList();

        this.WhenAnyValue(x => x.SelectedCategory)
            .Subscribe(item =>
            {
                if (item == null)
                {
                    ProductsCount = 0;
                    return;
                }

                var original = _originalCategories.FirstOrDefault(c => c.ProductCategoryId == item.Id);
                ProductsCount = original?.ProductsCount ?? 0;
            });
    }

    public void SetDialogCompletionSource(TaskCompletionSource<bool> tcs) => _tcs = tcs;
    public void SetDismissAction(Action dismiss) => _dismiss = dismiss;

    [ReactiveCommand]
    private async Task DeleteAsync()
    {
        if (SelectedCategory == null)
        {
            _toastService.ShowError(ConstantMessages.EmptySelectedItem);
            return;
        }

        if (ProductsCount > 0)
        {
            _toastService.ShowWarning(string.Format(ConstantMessages.CategoryCannotBeDeleted, ProductsCount));
            return;
        }

        var response = await _api.Delete(SelectedCategory.Id);
        if (response.IsSuccessStatusCode)
        {
            _dismiss?.Invoke();
            _tcs?.TrySetResult(true);
        }
    }

    [ReactiveCommand]
    private void Cancel()
    {
        _dismiss?.Invoke();
        _tcs?.TrySetResult(false);
    }
}