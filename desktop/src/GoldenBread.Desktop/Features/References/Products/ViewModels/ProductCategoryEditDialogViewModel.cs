using GoldenBread.Desktop.Features.References.Products.Models;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.ComponentModel.DataAnnotations;

namespace GoldenBread.Desktop.Features.References.Products.ViewModels;

public partial class ProductCategoryEditDialogViewModel : ViewModelBase, IDialogAware
{
    private readonly IProductCategoriesApi _api;
    private readonly ToastService _toastService;

    private TaskCompletionSource<bool>? _tcs;
    private Action? _dismiss;

    [Reactive] private List<ProductCategoryListItem> _categories;
    [Reactive] private ProductCategoryListItem? _selectedCategory;
    [Reactive][Required][StringLength(50, MinimumLength = 2)] string _name = null!;
    [Reactive][Required][RegularExpression(@"^#[0-9A-Fa-f]{6}$")] string _color = "#FF0000";

    public ProductCategoryEditDialogViewModel(
        IProductCategoriesApi api,
        ToastService toastService,
        List<ProductCategoryListItem> categories)
    {
        _categories = categories;
        _toastService = toastService;
        _api = api;

        this.WhenAnyValue(x => x.SelectedCategory)
            .Subscribe(value =>
            {
                if (value != null)
                {
                    Name = value.Name;
                    Color = value.Color;
                }
            });
    }

    public void SetDialogCompletionSource(TaskCompletionSource<bool> tcs) => _tcs = tcs;
    public void SetDismissAction(Action dismiss) => _dismiss = dismiss;

    [ReactiveCommand]
    private async Task SaveAsync()
    {
        if (SelectedCategory == null) { _toastService.ShowError(ConstantMessages.EmptySelectedItem); return; }
        if (HasErrors) { _toastService.ShowError(GetFirstError()); return; }
        var response = await _api.Update(new ProductCategoryDto(SelectedCategory.ProductCategoryId, Name, Color));
        if (response.IsSuccessStatusCode) { _dismiss?.Invoke(); _tcs?.TrySetResult(true); }
    }

    [ReactiveCommand] private void Cancel() { _dismiss?.Invoke(); _tcs?.TrySetResult(false); }
}