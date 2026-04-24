using ReactiveUI;
using ReactiveUI.SourceGenerators;
using SukiUI.Controls;

namespace GoldenBread.Desktop.UI.Common;

public partial class PageViewModelBase : ReactiveObject
{
    [Reactive] private string _displayName = string.Empty;
    [Reactive] private string _pageKey = string.Empty;
    [Reactive] private int _order;

    public PageViewModelBase(string displayName, string pageKey, int order = 0)
    {
        DisplayName = displayName;
        PageKey = pageKey;
        Order = order;
    }
}
