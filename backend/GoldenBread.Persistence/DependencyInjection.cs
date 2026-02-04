using GoldenBread.Domain.Enums;
using GoldenBread.Infrastructure.Data;
using GoldenBread.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using GoldenBread.Infrastructure.Services;
using GoldenBread.Application.Common.Abstractions.Repositories;
using GoldenBread.Application.Common.Abstractions.Services;
using GoldenBread.Application.Common.Abstractions;


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
            
        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();

        // Servises
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        // Other
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
