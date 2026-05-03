using Avalonia.Controls;
using GoldenBread.Desktop.Features.References.Employees.ViewModels;
using GoldenBread.Desktop.Features.References.Suppliers.ViewModels;
using ReactiveUI.Avalonia;

namespace GoldenBread.Desktop.Features.References.Suppliers.Views;

public partial class SuppliersHostPageView : ReactiveUserControl<SuppliersHostPageViewModel>
{
    public SuppliersHostPageView()
    {
        InitializeComponent();
    }
}