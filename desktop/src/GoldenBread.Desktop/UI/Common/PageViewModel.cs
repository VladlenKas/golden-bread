using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;

namespace GoldenBread.Desktop.UI.Common;

public partial class PageViewModel : ViewModelBase, IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new();

    [Reactive] private string _displayName = string.Empty;
    [Reactive] private string _pageKey = string.Empty;
    [Reactive] private int _order;

    protected PageViewModel()
    {
        this.WhenActivated(disposables =>
        {
            OnActivated();
            Disposable.Create(() =>
            {
                OnDeactivated();
            }).DisposeWith(disposables);
        });
    }   
            
    protected virtual void OnActivated() { }
    protected virtual void OnDeactivated() { }
}
