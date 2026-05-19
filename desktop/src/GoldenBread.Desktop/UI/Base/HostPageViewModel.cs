using GoldenBread.Desktop.Configuration.Models;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;

namespace GoldenBread.Desktop.UI.Common;

public abstract partial class HostPageViewModel : ReactiveObject, IActivatableViewModel
{
    [Reactive] private PageViewModel? _activePage;
    [Reactive] private string _displayName = string.Empty;
    [Reactive] private string _pageKey = string.Empty;
    [Reactive] private int _order;

    public ViewModelActivator Activator { get; } = new();
    public CrudPermissionConfig Permissions { get; set; } = new();

    protected HostPageViewModel()
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

    protected void NavigateTo(PageViewModel page) => ActivePage = page;

    protected virtual void OnActivated() { }
    protected virtual void OnDeactivated() { }
}