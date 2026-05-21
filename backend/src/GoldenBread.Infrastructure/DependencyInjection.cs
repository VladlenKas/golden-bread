using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Abstractions.Data.Services;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Common.Services;
using GoldenBread.Application.Common.Strategies.Employee;
using GoldenBread.Application.Common.Strategies.Product;
using GoldenBread.Domain.Enums;
using GoldenBread.Domain.Interfaces.Services;
using GoldenBread.Domain.Interfaces.Strategies;
using GoldenBread.Domain.Services;
using GoldenBread.Domain.ValueObjects;
using GoldenBread.Infrastructure.Data;
using GoldenBread.Infrastructure.Data.Repositories;
using GoldenBread.Infrastructure.Data.Services;
using GoldenBread.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GoldenBread.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<GoldenBreadContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("GoldenBread"), 
                npgsql => 
            {
                npgsql.MapEnum<AccountType>("account_type");
                npgsql.MapEnum<IngredientBatchStatus>("ingredient_batch_status");
                npgsql.MapEnum<IngredientUnit>("ingredient_unit");
                npgsql.MapEnum<OrderStatus>("order_status");
                npgsql.MapEnum<UserRole>("user_role");
                npgsql.MapEnum<VerificationStatus>("verification_status");
            }).UseSnakeCaseNamingConvention();
        });

        // EF
        services.AddScoped<IGoldenBreadContext, GoldenBreadContext>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        // Infra
        services.AddScoped<IFileStorage, FileStorage>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ICurrentAccountContext, CurrentAccountContext>();
        services.AddScoped<ICookieService, CookieService>();
        services.AddScoped<IDeliveryInvoiceGenerator, DeliveryInvoiceGenerator>();
        services.AddScoped<IUnitConversionService, UnitConversionService>();

        // Repositories
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductBatchRepository, ProductBatchRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IEmployeeTaskRepository, EmployeeTaskRepository>();
        services.AddScoped<IFavoriteRepository, FavoriteRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<ISupplierIngredientRepository, SupplierIngredientRepository>();
        services.AddScoped<IIngredientRepository, IngredientRepository>();

        // Data Services
        services.AddScoped<ICatalogQueryService, CatalogQueryService>();

        // Scheduling Module
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Ekaterinburg Standard Time");
        services.AddSingleton(timeZone);
        services.AddSingleton(WorkSchedule.Default());
        services.AddScoped<IWorkCalendar, WorkCalendar>();

        services.AddScoped<IEmployeeSelectionStrategy, ProportionalGreedyStrategy>();

        services.AddScoped<IOrderItemOrderingStrategy, FastestProductionStrategy>();
        services.AddScoped<IOrderItemOrderingStrategy, ShortestShelfLifeStrategy>();

        services.AddScoped<ScheduleTaskDistributor>();

        return services;
    }
}
