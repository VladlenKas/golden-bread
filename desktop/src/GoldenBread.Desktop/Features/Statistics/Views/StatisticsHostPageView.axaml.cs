using Avalonia.Controls;
using GoldenBread.Desktop.Features.Statistics.ViewModels;
using ReactiveUI.Avalonia;

namespace GoldenBread.Desktop.Features.Statistics.Views;

public partial class StatisticsHostPageView : ReactiveUserControl<StatisticsHostPageViewModel>
{
    public StatisticsHostPageView()
    {
        InitializeComponent();
    }
}