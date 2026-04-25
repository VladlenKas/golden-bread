using GoldenBread.Desktop.UI.Common;

namespace GoldenBread.Desktop.UI.Services.Views;

public interface IPageFactory
{
    PageViewModelBase? CreatePage(string pageKey);
}