using GoldenBread.Desktop.Configuration.Files;
using GoldenBread.Desktop.Configuration.Models;
using GoldenBread.Desktop.Features.Menu;
using GoldenBread.Desktop.Infrastructure.Auth;
using GoldenBread.Desktop.Infrastructure.Helpers;
using System.Data;

namespace GoldenBread.Desktop.Configuration.Services;

public sealed class MenuConfigService(CurrentUserStore userStore)
{
    private readonly RolesConfig _roles = JsonHelper.Load<RolesConfig>(AppSettings.RolesJson);
    private readonly PagesConfig _pages = JsonHelper.Load<PagesConfig>(AppSettings.PagesJson);  // ← новый файл

    /// <summary>
    /// Все страницы, доступные по правам роли пользователя (плоский список)
    /// </summary>
    public IReadOnlyList<PageMenuItem> GetSidebarPages()
    {
        var rolePages = GetRolePageKeys();

        return _pages.Pages
            .Where(p => rolePages.Contains(p.Key))
            .Select(p => new PageMenuItem
            {
                Key = p.Key,
                Title = p.Title,
                Icon = ParseHelper.ParseIcon(p.Icon),
                Order = p.Order
            })
            .OrderBy(p => p.Order)
            .ToList();
    }

    /// <summary>
    /// CRUD-права на страницу 
    /// </summary>
    public CrudPermissionConfig GetPagePermissions(string pageKey)
    {
        var roleConfig = _roles.Roles.FirstOrDefault(r => r.Key == userStore.Role.ToString());
        return roleConfig?.Pages.TryGetValue(pageKey, out var perm) == true
            ? perm : new CrudPermissionConfig();
    }

    /// <summary>
    /// Вспомогательный: все ключи страниц, доступных по правам роли пользователя
    /// </summary>
    public HashSet<string> GetRolePageKeys()
    {
        var roleConfig = _roles.Roles.FirstOrDefault(r => r.Key == userStore.Role.ToString());
        return roleConfig?.Pages.Keys.ToHashSet() ?? [];
    }
}