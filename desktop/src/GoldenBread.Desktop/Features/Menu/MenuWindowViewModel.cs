using Avalonia.Collections;
using Avalonia.Styling;
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

    [Reactive] private SectionMenuItem? _selectedSection;
    [Reactive] private HostPageViewModel? _activePage;
    [Reactive] private bool _isLoading = false;
    [Reactive] public ThemeVariant _baseTheme = ThemeVariant.Dark;
    [Reactive] public SukiBackgroundStyle _backgroundStyle = SukiBackgroundStyle.GradientSoft;

    public IAvaloniaReadOnlyList<SukiColorTheme> Themes => _theme.ColorThemes;
    public IAvaloniaReadOnlyList<SukiBackgroundStyle> BackgroundStyles { get; } =
        new AvaloniaList<SukiBackgroundStyle>(Enum.GetValues<SukiBackgroundStyle>());
    public IEnumerable<LocalizedBackground> LocalizedBackgrounds =>
        BackgroundStyles.Select(s => new LocalizedBackground(s));
    public IEnumerable<LocalizedTheme> LocalizedThemes =>
        Themes.Select(t => new LocalizedTheme(t));
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

        BaseTheme = _theme.ActiveBaseTheme;
        _theme.OnBaseThemeChanged += variant => BaseTheme = variant;

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

    public void ChangeTheme(SukiColorTheme theme) => _theme.ChangeColorTheme(theme);

    private void LoadSectionPages(SectionMenuItem section)
    {
        SectionPages.Clear();

        foreach (var page in section.Pages.OrderBy(p => p.Order))
        {
            var perm = _menuConfig.GetPagePermissions(page.Key);
            var vm = _pageFactory.GetHostPage(page.Key, page.Title, perm);

            if (vm is null) continue;

            SectionPages.Add(vm);
        }

        ActivePage = SectionPages.FirstOrDefault();
    }

    [ReactiveCommand]
    private void ToggleBaseTheme() => _theme.SwitchBaseTheme();

    [ReactiveCommand]
    private async Task LogoutAsync()
    {
        var tcs = _dialogService.ShowWarningQustion(ConstantMessages.LogoutConfirmDialog);

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
