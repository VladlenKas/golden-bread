using GoldenBread.Desktop.Configuration.Models;
using GoldenBread.Desktop.Features.Administration.Accounts;
using GoldenBread.Desktop.Features.Administration.Companies;
using GoldenBread.Desktop.Features.Administration.SystemUsers;
using GoldenBread.Desktop.Features.Orders.OrdersList;
using GoldenBread.Desktop.Features.Procurement.PurchasePositions;
using GoldenBread.Desktop.Features.Procurement.Warehouse;
using GoldenBread.Desktop.Features.Production.EmployeeTasks;
using GoldenBread.Desktop.Features.Production.ProductBatches;
using GoldenBread.Desktop.Features.Production.Recipes;
using GoldenBread.Desktop.Features.References.Employees.ViewModels;
using GoldenBread.Desktop.Features.References.Ingredients;
using GoldenBread.Desktop.Features.References.Products;
using GoldenBread.Desktop.Features.References.Suppliers.ViewModels;
using GoldenBread.Desktop.UI.Common;
using Microsoft.Extensions.DependencyInjection;
namespace GoldenBread.Desktop.UI.Services;

public sealed class PageFactory(IServiceProvider provider) 
{
    private static readonly Dictionary<string, Type> _pageTypes = new()
    {
        ["products"] = typeof(ProductsHostPageViewModel),
        ["ingredients"] = typeof(IngredientsHostPageViewModel),
        ["suppliers"] = typeof(SuppliersHostPageViewModel),
        ["employees"] = typeof(EmployeesHostPageViewModel),
        ["purchase_positions"] = typeof(PurchasePositionsHostPageViewModel),
        ["warehouse"] = typeof(WarehouseHostPageViewModel),
        ["recipes"] = typeof(RecipesHostPageViewModel),
        ["product_batches"] = typeof(ProductBatchesHostPageViewModel),
        ["employee_tasks"] = typeof(EmployeeTasksHostPageViewModel),
        ["orders_list"] = typeof(OrdersHostPageViewModel),
        ["system_users"] = typeof(SystemUsersHostPageViewModel),
        ["companies"] = typeof(CompaniesHostPageViewModel),
        ["accounts"] = typeof(AccountsHostPageViewModel),
    };

    public HostPageViewModel? GetHostPage(
        string pageKey, 
        string title,
        CrudPermissionConfig permissions)
    {
        if (!_pageTypes.TryGetValue(pageKey, out var type))
            return null;

        var page = (HostPageViewModel)provider.GetRequiredService(type);

        if (page != null)
        {
            page.PageKey = pageKey;  
            page.DisplayName = title;  
            page.Permissions = permissions;  
        }

        return page;
    }

    public T GetPage<T>() where T : PageViewModel
        => provider.GetRequiredService<T>();
}