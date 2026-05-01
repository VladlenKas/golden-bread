using GoldenBread.Desktop.Features.References.Employees.ViewModels;
using ReactiveUI.Avalonia;

namespace GoldenBread.Desktop.Features.References.Employees.Views;

public partial class EmployeesPageView 
    : ReactiveUserControl<EmployeesPageViewModel>
{
    public EmployeesPageView()
    {
        InitializeComponent();
    }
}