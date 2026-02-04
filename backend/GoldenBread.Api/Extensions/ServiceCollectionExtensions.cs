using GoldenBread.Api.Middlewares;

namespace GoldenBread.Api.Extensions;

public static class DependencyInjection 
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddValidatorsFromAssemblyContaining<Program>();

        services.AddControllers();
        services.AddOpenApi();

        return services;
    }   
}
