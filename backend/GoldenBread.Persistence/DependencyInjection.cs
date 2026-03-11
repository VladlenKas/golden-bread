using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Repositories;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Features.CompanyCart.Services;
using GoldenBread.Application.Features.CompanyOrder.Services;
using GoldenBread.Domain.Enums;
using GoldenBread.Domain.Interfaces.Services;
using GoldenBread.Domain.Services;
using GoldenBread.Infrastructure.Data;
using GoldenBread.Infrastructure.Data.Repositories;
using GoldenBread.Infrastructure.Jobs;
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
        services.AddScoped<IGoldenBreadContext, GoldenBreadContext>();

        // Infra services
        services.AddCommonServices();
        services.AddWebServices();

        return services;
    }

    private static void AddWebServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrentAccountContext, CurrentAccountContext>();
        services.AddScoped<ICookieService, CookieService>();
        services.AddScoped<IDeliveryDateCalculator, DeliveryDateCalculator>();
    }

    private static void AddCommonServices(this IServiceCollection services)
    {
        services.AddScoped<IFileStorage, FileStorage>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IUniquenessChecker, UniquenessChecker>();

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IEmployeeTaskRepository, EmployeeTaskRepository>();
        services.AddScoped<IIngredientReservationRepository, IngredientReservationRepository>();
        services.AddScoped<IOrderTariffRepository, OrderTariffRepository>();

        services.AddScoped<IIngredientReservationService, IngredientReservationService>();
        services.AddScoped<IEmployeeTaskDistributor, EmployeeTaskDistributor>();
        services.AddScoped<IProductionCalculator, ProductionCalculator>();
        services.AddScoped<IWorkScheduleCalculator, WorkScheduleCalculator>();
    }
}
