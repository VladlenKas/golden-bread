using FluentValidation;
using GoldenBread.Application;
using GoldenBread.Infrastructure;
using Scalar.AspNetCore;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace GoldenBread.Api.Extensions;

public static class DependencyInjection 
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<Program>();

        services.AddControllers();
        services.AddOpenApi();

        return services;
    }   
}
