using GoldenBread.Desktop.Features.References.Products.Models;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;
using System.ComponentModel.DataAnnotations;
using ReactiveUI.SourceGenerators;

namespace GoldenBread.Desktop.Features.References.Products.ViewModels;

public partial class ProductCategoryCreateDialogViewModel(
    IProductCategoriesApi api,
    ToastService toastService) : ViewModelBase, IDialogAware
{
    private TaskCompletionSource<bool>? _tcs;
    private Action? _dismiss;

    [Reactive][Required][StringLength(50, MinimumLength = 2)] string _name = null!;
    [Reactive][Required][RegularExpression(@"^#[0-9A-Fa-f]{6}$")] string _color = "#FF0000";

    public void SetDialogCompletionSource(TaskCompletionSource<bool> tcs) => _tcs = tcs;
    public void SetDismissAction(Action dismiss) => _dismiss = dismiss;

    [ReactiveCommand]
    private async Task SaveAsync()
    {
        if (HasErrors) { toastService.ShowError(GetFirstError()); return; }
        var response = await api.Create(new ProductCategoryDto(0, Name, Color));
        if (response.IsSuccessStatusCode) { _dismiss?.Invoke(); _tcs?.TrySetResult(true); }
    }

    [ReactiveCommand] private void Cancel() { _dismiss?.Invoke(); _tcs?.TrySetResult(false); }
}
