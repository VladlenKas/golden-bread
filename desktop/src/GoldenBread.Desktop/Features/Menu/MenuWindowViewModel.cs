using GoldenBread.Desktop.Configuration.Files;
using GoldenBread.Desktop.Configuration.Models;
using GoldenBread.Desktop.Configuration.Services;
using GoldenBread.Desktop.Features.Auth;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.Infrastructure.Auth;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;
using Material.Icons;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace GoldenBread.Desktop.Features.Menu;

public partial class MenuWindowViewModel : ViewModelBase
{
    private readonly IAuthApi _authApi;
    private readonly SessionStorage _sessionStorage;
    private readonly CurrentUserStore _userStore;
    private readonly MenuConfigService _menuConfigService;
    private readonly PageFactory _pageFactory;
    private readonly WindowService _windowService;
    private readonly DialogService _dialogService;

    private readonly SectionsConfig _sectionsConfig;
    private readonly RolesConfig _rolesConfig;

    [Reactive] private SectionMenuItem? _selectedSection;
    [Reactive] private PageViewModel? _activePage;
    [Reactive] private bool _isLoading = false;

    public ISukiDialogManager SukiDialogManager { get; } 
    public ISukiToastManager SukiToastManager { get; }
    public ObservableCollection<SectionMenuItem> SidebarSections { get; } = new();
    public ObservableCollection<PageViewModel> SectionPages { get; } = new();

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
        _menuConfigService = menuConfigService;
        _pageFactory = pageFactory;
        _windowService = windowService;
        _authApi = authApi;
        _sessionStorage = sessionStorage;
        SukiDialogManager = sukiDialogManager;
        SukiToastManager = sukiToastManager;
        _dialogService = dialogService;

        // Загружаем конфигурационные файлы
        _sectionsConfig = _menuConfigService.LoadSections(AppSettings.SectionsJson);
        _rolesConfig = _menuConfigService.LoadRoles(AppSettings.RolesJson);

        // При смене выбранного раздела – загружаем его страницы и выбираем первую
        this.WhenAnyValue(x => x.SelectedSection)
            .WhereNotNull()
            .Subscribe(section => LoadSectionPages(section));

        // Отображаем разделы меню
        Initialize();
    }

    private void Initialize()
    {
        var role = _userStore.Role;
        if (role is null) return;

        // Получаем видимые разделы 
        var visibleSections = _menuConfigService.GetSidebarSections(
            role.Value,
            _sectionsConfig,
            _rolesConfig);

        // Преобразуем в элементы меню, сортируем и устанавливаем иконки
        var menuItems = visibleSections
            .Select(section =>
            {
                // Парсим строку иконки в MaterialIconKind
                _ = Enum.TryParse<MaterialIconKind>(section.Icon, out var iconKind);
                return new SectionMenuItem
                {
                    Key = section.Key,
                    Title = section.Title,
                    Icon = iconKind,
                    Order = section.Order
                };
            })
            .OrderBy(s => s.Order)
            .ToList();

        SidebarSections.Clear();

        foreach (var item in menuItems)
            SidebarSections.Add(item);

        // Выбираем первый раздел по умолчанию
        SelectedSection = SidebarSections.FirstOrDefault();
    }

    private void LoadSectionPages(SectionMenuItem section)
    {
        if (_sectionsConfig is null || _rolesConfig is null) return;

        var role = _userStore.Role;
        if (role is null) return;

        var pageConfigs = _menuConfigService.GetPages(
            role.Value,
            _sectionsConfig,
            section.Key,
            _rolesConfig);

        var pageViewModels = new List<PageViewModel>();
        foreach (var cfg in pageConfigs)
        {
            var page = _pageFactory.CreateMainPage(cfg.Key);
            if (page is not null)
            {
                page.DisplayName = cfg.Title;
                page.Order = cfg.Order;
                page.PageKey = cfg.Key;
                pageViewModels.Add(page);
            }
        }

        SectionPages.Clear();
        foreach (var pageVm in pageViewModels)
            SectionPages.Add(pageVm);

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
