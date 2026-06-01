using Avalonia.Collections;
using Avalonia.Styling;
using DynamicData.Binding;
using GoldenBread.Desktop.Configuration.Services;
using GoldenBread.Desktop.Features.Auth;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.Infrastructure.Auth;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Helpers;
using GoldenBread.Desktop.UI.Services;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using SukiUI;
using SukiUI.Dialogs;
using SukiUI.Enums;
using SukiUI.Models;
using SukiUI.Toasts;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace GoldenBread.Desktop.Features.Menu;

public partial class MenuWindowViewModel : ViewModelBase
{
    private readonly SukiTheme _theme = SukiTheme.GetInstance();
    private readonly IAuthApi _authApi;
    private readonly SessionStorage _sessionStorage;
    private readonly CurrentUserStore _userStore;
    private readonly MenuConfigService _menuConfig;
    private readonly PageFactory _pageFactory;
    private readonly WindowService _windowService;
    private readonly DialogService _dialogService;

    [Reactive] private string? _userData;
    [Reactive] private string? _userSessionData;
    [Reactive] private PageMenuItem? _selectedPage;
    [Reactive] private HostPageViewModel? _activePage;
    [Reactive] private bool _isLoading = false;
    [Reactive] private string? _baseThemeHeader;
    [Reactive] public ThemeVariant _baseTheme = ThemeVariant.Dark;
    [Reactive] public SukiBackgroundStyle _backgroundStyle = SukiBackgroundStyle.GradientSoft;

    public IAvaloniaReadOnlyList<SukiColorTheme> Themes => _theme.ColorThemes;
    public IAvaloniaReadOnlyList<SukiBackgroundStyle> BackgroundStyles { get; } =
        new AvaloniaList<SukiBackgroundStyle>(Enum.GetValues<SukiBackgroundStyle>());
    public IEnumerable<LocalizedTheme> LocalizedThemes =>
        _theme.ColorThemes
            .Where(LocalizedTheme.IsAllowed)
            .Select(t => new LocalizedTheme(t));
    public IEnumerable<LocalizedBackground> LocalizedBackgrounds =>
        Enum.GetValues<SukiBackgroundStyle>()
            .Where(LocalizedBackground.IsAllowed)
            .Select(s => new LocalizedBackground(s))
            .OrderBy(s => s.DisplayName); 

    public ISukiDialogManager SukiDialogManager { get; } 
    public ISukiToastManager SukiToastManager { get; }
    public ObservableCollection<PageMenuItem> SidebarPages { get; } = new();

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

        UserData = userStore.Info;
        UserSessionData = userStore.SessionInfo;
        BaseTheme = _theme.ActiveBaseTheme;
        BaseThemeHeader = _theme.ActiveBaseTheme.Equals(ThemeVariant.Dark) ? "Темная" : "Светлая";

        _theme.OnBaseThemeChanged += variant =>
        {
            BaseTheme = variant;
            BaseThemeHeader = variant.Equals(ThemeVariant.Dark) ? "Темная" : "Светлая";
        };

        // При смене выбранного раздела – загружаем его страницы и выбираем первую
        this.WhenAnyValue(x => x.SelectedPage)
            .WhereNotNull()
            .Subscribe(page => LoadPage(page));

        var pages = _menuConfig.GetSidebarPages();
        SidebarPages.Clear();
        foreach (var p in pages)
            SidebarPages.Add(p);

        SelectedPage = SidebarPages.FirstOrDefault();
    }

    private void LoadPage(PageMenuItem page)
    {
        var perm = _menuConfig.GetPagePermissions(page.Key);
        var vm = _pageFactory.GetHostPage(page.Key, page.Title, perm);

        if (vm is null) return;

        ActivePage = vm;
    }

    public void ChangeTheme(SukiColorTheme theme) => _theme.ChangeColorTheme(theme);

    [ReactiveCommand]
    private void ToggleBaseTheme() => _theme.SwitchBaseTheme();

    [ReactiveCommand]
    private async Task LogoutAsync()
    {
        var tcs = _dialogService.ShowWarningQuestion(ConstantMessages.LogoutConfirmDialog);

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
            _dialogService.ShowError(ConstantMessages.ExceptionDialog);
        }
        finally
        {
            IsLoading = false;
        }
    }
}
