using GoldenBread.Desktop.Configuration.Models;
using GoldenBread.Desktop.Features.Administration.Companies.ViewModels;
using GoldenBread.Desktop.Features.Administration.Users.ViewModels;
using GoldenBread.Desktop.Features.Procurement.PurchasePositions.ViewModels;
using GoldenBread.Desktop.Features.Procurement.Warehouse;
using GoldenBread.Desktop.Features.Production.OrdersList.ViewModels;
using GoldenBread.Desktop.Features.References.Employees.ViewModels;
using GoldenBread.Desktop.Features.References.Products.ViewModels;
using GoldenBread.Desktop.Features.References.Suppliers.ViewModels;
using GoldenBread.Desktop.UI.Common;
using Microsoft.Extensions.DependencyInjection;
namespace GoldenBread.Desktop.UI.Services;

public sealed class PageFactory(IServiceProvider provider) 
{
    private static readonly Dictionary<string, Type> _pageTypes = new()
    {
        ["products"] = typeof(ProductsHostPageViewModel),
        ["system_users"] = typeof(UsersHostPageViewModel),
        ["companies"] = typeof(CompaniesHostPageViewModel),
        ["suppliers"] = typeof(SuppliersHostPageViewModel),
        ["employees"] = typeof(EmployeesHostPageViewModel),
        ["purchase_positions"] = typeof(PurchasePositionsHostPageViewModel),
        ["warehouse"] = typeof(WarehouseHostPageViewModel),
        ["orders"] = typeof(OrdersHostPageViewModel),
        //["analytics"] = typeof(AnalyticsHostPageViewModel),
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