using GoldenBread.Desktop.Configuration.Models;
using GoldenBread.Desktop.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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
            .Where(section => section.ShowInSidebar)
            .Select(section => new AppSectionConfig
            {
                Key = section.Key,
                Title = section.Title,
                Icon = section.Icon,
                Route = section.Route,
                ShowInSidebar = section.ShowInSidebar,
                Order = section.Order,
                Pages = section.Pages
                    .Where(page => roleConfig.Permissions.TryGetValue(page.Key, out var perm) && perm.View)
                    .OrderBy(page => page.Order)
                    .ToList()
            })
            .Where(section => section.Pages.Count > 0)
            .OrderBy(section => section.Order)
            .ToList();
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

        if (!roleConfig.Permissions.TryGetValue(pageKey, out var permission))
            return new CrudPermissionConfig();

        return permission;
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
}