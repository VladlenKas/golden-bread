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
using GoldenBread.Desktop.Features.References.Suppliers;
using GoldenBread.Desktop.UI.Common;
using Microsoft.Extensions.DependencyInjection;
namespace GoldenBread.Desktop.UI.Services;

public class PageFactory(IServiceProvider provider) 
{
    private readonly Dictionary<string, Type> _pageTypes = new()
    {
        { "products", typeof(ProductsPageViewModel) },
        { "ingredients", typeof(IngredientsPageViewModel) },
        { "suppliers", typeof(SuppliersPageViewModel) },
        { "employees", typeof(EmployeesPageViewModel) },
        { "purchase_positions", typeof(PurchasePositionsPageViewModel) },
        { "warehouse", typeof(WarehousePageViewModel) },
        { "recipes", typeof(RecipesPageViewModel) },
        { "product_batches", typeof(ProductBatchesPageViewModel) },
        { "employee_tasks", typeof(EmployeeTasksPageViewModel) },
        { "orders_list", typeof(OrdersListPageViewModel) },
        { "system_users", typeof(SystemUsersPageViewModel) },
        { "companies", typeof(CompaniesPageViewModel) },
        { "accounts", typeof(AccountsPageViewModel) },
    };

    public PageViewModel? CreateMainPage(string pageKey)
    {
        if (_pageTypes.TryGetValue(pageKey, out var type))
            return (PageViewModel)provider.GetRequiredService(type);
        return null;
    }

    public T Create<T>() where T : PageViewModel => provider.GetRequiredService<T>();
}