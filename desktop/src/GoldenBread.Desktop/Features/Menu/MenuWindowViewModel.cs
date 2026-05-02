using GoldenBread.Desktop.Configuration.Services;
using GoldenBread.Desktop.Features.Auth;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.Infrastructure.Auth;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using System.Collections.ObjectModel;

namespace GoldenBread.Desktop.Features.Menu;

public partial class MenuWindowViewModel : ViewModelBase
{
    private readonly IAuthApi _authApi;
    private readonly SessionStorage _sessionStorage;
    private readonly CurrentUserStore _userStore;
    private readonly MenuConfigService _menuConfig;
    private readonly PageFactory _pageFactory;
    private readonly WindowService _windowService;
    private readonly DialogService _dialogService;

    [Reactive] private SectionMenuItem? _selectedSection;
    [Reactive] private HostPageViewModel? _activePage;
    [Reactive] private bool _isLoading = false;

    public ISukiDialogManager SukiDialogManager { get; } 
    public ISukiToastManager SukiToastManager { get; }
    public ObservableCollection<SectionMenuItem> SidebarSections { get; } = new();
    public ObservableCollection<HostPageViewModel> SectionPages { get; } = new();

    public MenuWindowViewModel(
        ISukiDialogManager sukiDialogManager,
        ISukiToastManager sukiToastManager,
        DialogService dialogService,
        IAuthApi authApi,
        SessionStorage sessionStorage,
        PageFactory pageFactory,
        CurrentUserStore userStore,
        MenuConfigService menuConfigService,
        WindowService windowService)
    {
        _userStore = userStore;
        _menuConfig = menuConfigService;
        _pageFactory = pageFactory;
        _windowService = windowService;
        _authApi = authApi;
        _sessionStorage = sessionStorage;
        SukiDialogManager = sukiDialogManager;
        SukiToastManager = sukiToastManager;
        _dialogService = dialogService;

        // При смене выбранного раздела – загружаем его страницы и выбираем первую
        this.WhenAnyValue(x => x.SelectedSection)
            .WhereNotNull()
            .Subscribe(section => LoadSectionPages(section));

        // Отображаем разделы меню
        var sections = _menuConfig.GetSidebarSectionsWithPages();

        SidebarSections.Clear();
        foreach (var item in sections)
            SidebarSections.Add(item);

        SelectedSection = SidebarSections.FirstOrDefault();
    }

    private void LoadSectionPages(SectionMenuItem section)
    {
        SectionPages.Clear();

        foreach (var page in section.Pages.OrderBy(p => p.Order))
        {
            var vm = _pageFactory.GetHostPage(page.Key);
            if (vm is null) continue;

            vm.DisplayName = page.Title;
            vm.PageKey = page.Key;
            vm.Permissions = _menuConfig.GetPagePermissions(page.Key);

            SectionPages.Add(vm);
        }

        ActivePage = SectionPages.FirstOrDefault();
    }

    [ReactiveCommand]
    private async Task LogoutAsync()
    {
        var tcs = _dialogService.ShowQustion("Вы действительно хотите выйти?");

        bool confirmed = await tcs.Task;

        if (!confirmed)
            return;

        try
        {
            IsLoading = true;

            await _authApi.Logout();
            _userStore.Logout();
            _sessionStorage.Clear();

            _windowService.ShowWindow<AuthWindowView, AuthWindowViewModel>();
            _windowService.CloseWindow(this);
        }
        catch
        {
            _dialogService.ShowError(ConstantMessages.ErrorException);
        }
        finally
        {
            IsLoading = false;
        }
    }
}
