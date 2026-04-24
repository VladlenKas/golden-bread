using GoldenBread.Desktop.Configuration.Models;
using GoldenBread.Desktop.Infrastructure.Common;
using System.Data;
using System.Text.Json;

namespace GoldenBread.Desktop.Configuration.Services;

public sealed class AuthorizationConfigService : IAuthorizationConfigService
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public SectionsConfig LoadSections(string path)
    {
        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<SectionsConfig>(json, _jsonOptions)
               ?? new SectionsConfig();
    }

    public RolesConfig LoadRoles(string path)
    {
        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<RolesConfig>(json, _jsonOptions)
               ?? new RolesConfig();
    }

    public IReadOnlyList<AppSectionConfig> GetSidebarSections(
        UserRole role,
        SectionsConfig sectionsConfig,
        RolesConfig rolesConfig)
    {
        var roleConfig = rolesConfig.Roles
            .FirstOrDefault(r => r.Key == role.ToString());

        if (roleConfig is null)
            return Array.Empty<AppSectionConfig>();

        return sectionsConfig.Sections
            .Where(section => 
                section.Pages.Any(page =>
                    GetPagePermissions(role, page.Key, rolesConfig).View))
            .Select(section => new AppSectionConfig
            {
                Key = section.Key,
                Title = section.Title,
                Icon = section.Icon,
                ShowInSidebar = true,
                Order = section.Order,
            })
            .OrderBy(section => section.Order)
            .ToList();
    }
    
    public bool CanAccessSection(
        UserRole role,
        string sectionKey,
        SectionsConfig sectionsConfig,
        RolesConfig rolesConfig)
    {
        var roleConfig = rolesConfig.Roles
            .FirstOrDefault(r => r.Key == role.ToString());

        var section = sectionsConfig.Sections.FirstOrDefault(s => s.Key == sectionKey);
        if (section is null) return false;

        return section.Pages.Any(page =>
            GetPagePermissions(role, page.Key, rolesConfig).View);
    }

    public CrudPermissionConfig GetPagePermissions(
        UserRole role,
        string pageKey,
        RolesConfig rolesConfig)
    {
        var roleConfig = rolesConfig.Roles
            .FirstOrDefault(r => r.Key == role.ToString());

        if (roleConfig is null)
            return new CrudPermissionConfig();

        if (!roleConfig.Pages.TryGetValue(pageKey, out var permission))
            return new CrudPermissionConfig();

        return permission;
    }
}