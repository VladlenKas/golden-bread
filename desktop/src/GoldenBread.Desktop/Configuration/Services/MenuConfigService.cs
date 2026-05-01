using Avalonia.Platform;
using GoldenBread.Desktop.Configuration.Models;
using GoldenBread.Desktop.Features.Common.Models;
using System.Data;
using System.Text.Json;

namespace GoldenBread.Desktop.Configuration.Services;

public sealed class MenuConfigService
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public SectionsConfig LoadSections(string path)
    {
        var uri = new Uri(path);

        using var stream = AssetLoader.Open(uri);
        using var reader = new StreamReader(stream);

        var json = reader.ReadToEnd();
        return JsonSerializer.Deserialize<SectionsConfig>(json)!;
    }

    public RolesConfig LoadRoles(string path)
    {
        var uri = new Uri(path);

        using var stream = AssetLoader.Open(uri);
        using var reader = new StreamReader(stream);

        var json = reader.ReadToEnd();
        return JsonSerializer.Deserialize<RolesConfig>(json)!;
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

    public IReadOnlyList<AppPageConfig> GetPages(
        UserRole role,
        SectionsConfig sectionsConfig,
        string sectionKey,
        RolesConfig rolesConfig)
    {
        var roleConfig = rolesConfig.Roles
            .FirstOrDefault(r => r.Key == role.ToString());

        if (roleConfig is null)
            return Array.Empty<AppPageConfig>();

        var fullSection = sectionsConfig.Sections
            .FirstOrDefault(s => s.Key == sectionKey);

        if (fullSection is null)
            return Array.Empty<AppPageConfig>();

        return fullSection.Pages
            .Where(page =>
                GetPagePermissions(role, page.Key, rolesConfig).View)
            .OrderBy(page => page.Order)
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