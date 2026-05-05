using GoldenBread.Desktop.Features.Administration.Users.ViewModels;
using ReactiveUI.Avalonia;

namespace GoldenBread.Desktop.Features.Administration.Users.Views;

public partial class UsersHostPageView : ReactiveUserControl<UsersHostPageViewModel>
{
    public UsersHostPageView()
    {
        InitializeComponent();
    }
}