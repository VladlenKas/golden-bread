using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Collections.ObjectModel;

namespace GoldenBread.Desktop.UI.Common;

public partial class SectionViewModelBase : ReactiveObject
{
    [Reactive] private string _key = string.Empty;
    [Reactive] private string _displayName = string.Empty;
    [Reactive] private string _icon = "Home";
    [Reactive] private int _order;
    [Reactive] private PageViewModelBase? _activePage;

    public ObservableCollection<PageViewModelBase> Pages { get; } = new();

    public SectionViewModelBase(string key, string displayName, string icon, int order)
    {
        Key = key;
        DisplayName = displayName;
        Icon = icon;
        Order = order;
    }

    public PageViewModelBase? GetFirstPage() => Pages.OrderBy(p => p.Order).FirstOrDefault();
}
