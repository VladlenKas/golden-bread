using GoldenBread.Domain.Enums;
using GoldenBread.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GoldenBread.Infrastructure.Services;
using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Features.CompanyCart.Services;

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
    }
}
