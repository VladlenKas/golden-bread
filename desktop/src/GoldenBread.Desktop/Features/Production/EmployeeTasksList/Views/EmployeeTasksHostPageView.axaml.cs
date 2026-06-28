using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GoldenBread.Desktop.Features.Production.EmployeeTasksList.ViewModels;
using ReactiveUI.Avalonia;

namespace GoldenBread.Desktop.Features.Production.EmployeeTasksList.Views;

public partial class EmployeeTasksHostPageView : ReactiveUserControl<EmployeeTasksHostPageViewModel>
{
    public EmployeeTasksHostPageView()
    {
        InitializeComponent();
    }
}