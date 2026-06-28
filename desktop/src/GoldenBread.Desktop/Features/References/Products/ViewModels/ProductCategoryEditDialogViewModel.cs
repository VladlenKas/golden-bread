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

    [Reactive] private List<ProductCategoryAutoCompleteItem> _categories = new();
    [Reactive] private ProductCategoryAutoCompleteItem? _selectedCategory;

    [Reactive]
    [Required(ErrorMessage = "Название обязательно")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Название должно быть от 2 до 50 символов")]
    [RegularExpression(ConstantRegularExpressions.Name, ErrorMessage = ConstantMessages.NameFormatValidation)]
    string _name = null!;

    [Reactive]
    [Required(ErrorMessage = "Цвет обязателен")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Цвет должен быть ровно 6 символов")]
    [RegularExpression(@"^[0-9A-Fa-f]{6}$", ErrorMessage = "Введите цвет в формате HEX (6 символов)")]
    string _color = "FF0000";

    public ProductCategoryEditDialogViewModel(
        IProductCategoriesApi api,
        ToastService toastService,
        List<ProductCategoryListItem> categories)
    {
        _api = api;
        _toastService = toastService;

        _categories = categories
            .Select(c => new ProductCategoryAutoCompleteItem(c.ProductCategoryId, c.Name, c.Color))
            .ToList();

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
        if (SelectedCategory == null)
        {
            _toastService.ShowError(ConstantMessages.EmptySelectedItem);
            return;
        }

        if (HasErrors)
        {
            _toastService.ShowError(GetFirstError());
            return;
        }

        var response = await _api.Update(new ProductCategoryDto(SelectedCategory.Id, Name, Color));
        if (response.IsSuccessStatusCode)
        {
            _dismiss?.Invoke();
            _tcs?.TrySetResult(true);
        }
        else
        {
            var msg = response.Error != null
                ? GoldenBreadApiClient.GetErrorMessage(response.Error)
                : null;

            _toastService.ShowError(msg);
            return;
        }
    }

    [ReactiveCommand]
    private void Cancel()
    {
        _dismiss?.Invoke();
        _tcs?.TrySetResult(false);
    }
}