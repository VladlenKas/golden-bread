using ReactiveUI.SourceGenerators;
using SukiUI.Controls;

namespace GoldenBread.Desktop.UI.Common;

public abstract partial class StackPageViewModel : PageViewModel
{
    [Reactive] private PageViewModel? _activePage;

    protected void NavigateTo(PageViewModel page)
    {
        ActivePage = page;
    }
}