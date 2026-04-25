using GoldenBread.Desktop.Configuration.Models;
using GoldenBread.Desktop.Configuration.Services;
using GoldenBread.Desktop.Features.Auth;
using GoldenBread.Desktop.Infrastructure.Api.Clients;
using GoldenBread.Desktop.Infrastructure.Auth;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services.Dialogs;
using GoldenBread.Desktop.UI.Services.Views;
using GoldenBread.Desktop.UI.Services.Windows;
using Material.Icons;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace GoldenBread.Desktop.Features.Menu;

public partial class MenuWindowViewModel : ViewModelBase
{
    private readonly IAuthApi _authApi;
    private readonly ISessionStorage _sessionStorage;
    private readonly ICurrentUserStore _userStore;
    private readonly IMenuConfigService _menuConfigService;
    private readonly IPageFactory _pageFactory;
    private readonly IDialogService _dialogService;
    private readonly IWindowService _windowService;

    private readonly SectionsConfig _sectionsConfig;
    private readonly RolesConfig _rolesConfig;

    // Коллекции для привязки
    [Reactive] private SectionMenuItem? _selectedSection;
    [Reactive] private PageViewModelBase? _activePage;
    [Reactive] private bool _isLoading = false;

    public ObservableCollection<SectionMenuItem> SidebarSections { get; } = new();
    public ObservableCollection<PageViewModelBase> SectionPages { get; } = new();

    public MenuWindowViewModel(
        IAuthApi authApi,
        ISessionStorage sessionStorage,
        IDialogService dialogService,
        IPageFactory pageFactory,
        ICurrentUserStore userStore,
        IMenuConfigService menuConfigService,
        IWindowService windowService)
    {
        _userStore = userStore;
        _menuConfigService = menuConfigService;
        _pageFactory = pageFactory;
        _dialogService = dialogService;
        _windowService = windowService;
        _authApi = authApi;
        _sessionStorage = sessionStorage;

        // Загружаем конфигурационные файлы
        _sectionsConfig = _menuConfigService.LoadSections(Paths.SectionsJson);
        _rolesConfig = _menuConfigService.LoadRoles(Paths.RolesJson);

        // При смене выбранного раздела – загружаем его страницы и выбираем первую
        this.WhenAnyValue(x => x.SelectedSection)
            .WhereNotNull()
            .Subscribe(section => LoadSectionPages(section));

        Initialize();
    }

    [ReactiveCommand]
    private async Task LogoutAsync()
    {
        var tcs = _dialogService.ShowQustion(
            DialogManager, "Вы действительно хотите выйти?");

        bool confirmed = await tcs.Task;

        if (!confirmed)
            return;

        try
        {
            IsLoading = true;

            var result = await _authApi.Logout();

            if (result.IsSuccessStatusCode)
            {
                Debug.WriteLine("Okk");
            }
            else
            {
                Debug.WriteLine("[LogoutAsync] " + result.IsSuccessful);
                Debug.WriteLine("[LogoutAsync] " + result.Headers);
                Debug.WriteLine("[LogoutAsync] " + result.ContentHeaders);
                Debug.WriteLine("[LogoutAsync] " + result.Content);
                Debug.WriteLine("[LogoutAsync] " + result.Error);
                Debug.WriteLine("[LogoutAsync] " + result.RequestMessage);
                Debug.WriteLine("[LogoutAsync] " + result.StatusCode);
            }

            _userStore.Logout();
            _sessionStorage.Clear();

            _windowService.ShowWindow<AuthWindowView, AuthWindowViewModel>();
            _windowService.CloseWindow(this);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            _dialogService.ShowError(DialogManager, DialogMessages.ErrorException);
        }
        finally
        {
            IsLoading = false;
        }
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

        var pageViewModels = new List<PageViewModelBase>();
        foreach (var cfg in pageConfigs)
        {
            var page = _pageFactory.CreatePage(cfg.Key);
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
}
