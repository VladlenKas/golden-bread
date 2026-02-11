using GoldenBread.Domain.Enums;
using GoldenBread.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GoldenBread.Infrastructure.Services;
using GoldenBread.Application.Common.Abstractions.Services;
using GoldenBread.Application.Common.Abstractions.Data;

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

        // Servises
        services.AddScoped<ICookieService, CookieService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }
}
