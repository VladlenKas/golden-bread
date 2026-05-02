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
    private readonly SectionsConfig _sections = JsonHelper.Load<SectionsConfig>(AppSettings.SectionsJson);

    /// <summary>
    /// Все секции со страницами, доступные по правам роли пользователя
    /// </summary>
    public IReadOnlyList<SectionMenuItem> GetSidebarSectionsWithPages()
    {
        var rolePages = GetRolePageKeys();

        return _sections.Sections
            .Where(s => s.Pages.Any(p => rolePages.Contains(p.Key)))
            .Select(s => new SectionMenuItem
            {
                Key = s.Key,
                Title = s.Title,
                Icon = ParseHelper.ParseIcon(s.Icon),
                Order = s.Order,
                Pages = s.Pages
                    .Where(p => rolePages.Contains(p.Key))
                    .Select(p => new PageMenuItem
                    {
                        Key = p.Key,
                        Title = p.Title,
                        Order = p.Order
                    })
                    .ToList()
            })
            .OrderBy(s => s.Order)
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