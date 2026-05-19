using GoldenBread.Desktop.Features.References.Products.Models;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;
using ReactiveUI.SourceGenerators;

namespace GoldenBread.Desktop.Features.References.Products.ViewModels;

public partial class ProductCategoryDeleteDialogViewModel(
    IProductCategoriesApi api,
    ToastService toastService,
    List<ProductCategoryListItem> categories) : ViewModelBase, IDialogAware
{
    private TaskCompletionSource<bool>? _tcs;
    private Action? _dismiss;

    [Reactive] private List<ProductCategoryListItem> _categories = categories;
    [Reactive] private ProductCategoryListItem? _selectedCategory;

    public void SetDialogCompletionSource(TaskCompletionSource<bool> tcs) => _tcs = tcs;
    public void SetDismissAction(Action dismiss) => _dismiss = dismiss;

    [ReactiveCommand]
    private async Task DeleteAsync()
    {
        if (SelectedCategory == null) { toastService.ShowError(ConstantMessages.EmptySelectedItem); return; }
        if (SelectedCategory.ProductsCount > 0)
        {
            toastService.ShowWarning(string.Format(ConstantMessages.CategoryCannotBeDeleted, SelectedCategory.ProductsCount));
            return;
        }
        var response = await api.Delete(SelectedCategory.ProductCategoryId);
        if (response.IsSuccessStatusCode) { _dismiss?.Invoke(); _tcs?.TrySetResult(true); }
    }

    [ReactiveCommand] private void Cancel() { _dismiss?.Invoke(); _tcs?.TrySetResult(false); }
}