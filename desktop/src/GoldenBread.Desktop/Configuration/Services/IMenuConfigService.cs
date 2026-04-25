using GoldenBread.Desktop.Configuration.Models;
using GoldenBread.Desktop.Infrastructure.Common;
using System.Collections.Generic;

namespace GoldenBread.Desktop.Configuration.Services;

public interface IMenuConfigService
{
    /// <summary>
    /// Загружает конфигурацию разделов из JSON-файла.
    /// </summary>
    /// <param name="path">Путь к JSON-файлу с разделами.</param>
    /// <returns>Конфигурация разделов приложения.</returns>
    SectionsConfig LoadSections(string path);

    /// <summary>
    /// Загружает конфигурацию ролей и прав из JSON-файла.
    /// </summary>
    /// <param name="path">Путь к JSON-файлу с ролями.</param>
    /// <returns>Конфигурация ролей и прав.</returns>
    RolesConfig LoadRoles(string path);

    /// <summary>
    /// Возвращает разделы для sidebar'а, фильтруя по правам пользователя.
    /// Включает только разделы, в которых есть хотя бы одна страница с правом View.
    /// </summary>
    /// <param name="currentUser">Текущий пользователь.</param>
    /// <param name="sectionsConfig">Конфигурация всех разделов.</param>
    /// <param name="rolesConfig">Конфигурация ролей и прав.</param>
    /// <returns>Отфильтрованный список разделов с видимыми страницами.</returns>
    IReadOnlyList<AppSectionConfig> GetSidebarSections(
        UserRole role,
        SectionsConfig sectionsConfig,
        RolesConfig rolesConfig);

    /// <summary>
    /// Возвращает страницы раздела, доступные пользователю (с правом View).
    /// </summary>
    /// <param name="role">Роль пользователя.</param>
    /// <param name="sectionsConfig">Конфигурация всех разделов.</param>
    /// <param name="sectionKey">Ключ раздела.</param>
    /// <param name="rolesConfig">Конфигурация ролей и прав.</param>
    /// <returns>Список конфигураций страниц, отсортированный по Order.</returns>
    IReadOnlyList<AppPageConfig> GetPages(
        UserRole role,
        SectionsConfig sectionsConfig,
        string sectionKey,
        RolesConfig rolesConfig);

    /// <summary>
    /// Возвращает CRUD-права пользователя на конкретную страницу.
    /// </summary>
    /// <param name="currentUser">Текущий пользователь.</param>
    /// <param name="pageKey">Ключ страницы.</param>
    /// <param name="rolesConfig">Конфигурация ролей и прав.</param>
    /// <returns>Права доступа (View, Create, Update, Delete).</returns>
    CrudPermissionConfig GetPagePermissions(
        UserRole role,
        string pageKey,
        RolesConfig rolesConfig);

    /// <summary>
    /// Проверяет, есть ли у пользователя доступ к разделу 
    /// (хотя бы на одну страницу внутри раздела).
    /// </summary>
    /// <param name="currentUser">Текущий пользователь.</param>
    /// <param name="sectionKey">Ключ раздела.</param>
    /// <param name="sectionsConfig">Конфигурация всех разделов.</param>
    /// <param name="rolesConfig">Конфигурация ролей и прав.</param>
    /// <returns>true, если есть доступ хотя бы к одной странице раздела.</returns>
    bool CanAccessSection(
        UserRole role,
        string sectionKey,
        SectionsConfig sectionsConfig,
        RolesConfig rolesConfig);
}