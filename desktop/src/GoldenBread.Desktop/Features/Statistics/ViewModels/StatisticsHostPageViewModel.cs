using GoldenBread.Desktop.Features.Statistics.Views;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;

namespace GoldenBread.Desktop.Features.Statistics.ViewModels;

public partial class StatisticsHostPageViewModel : HostPageViewModel
{
    private readonly StatisticsListPageViewModel _listPage;

    public StatisticsHostPageViewModel(PageFactory factory)
    {
        _listPage = factory.GetPage<StatisticsListPageViewModel>();
    }

    protected override void OnActivated()
    {
        _listPage.Permissions = this.Permissions;
        _listPage.RefreshCommand.Execute();
        NavigateTo(_listPage);
    }

    protected override void OnDeactivated() { }
}
