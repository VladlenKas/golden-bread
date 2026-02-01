using FluentValidation;
using GoldenBread.Application.Common.Behaviors;
using MediatR;
using System.Reflection;

namespace GoldenBread.Api.Extensions;

public static class DependencyInjection 
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        //services.AddMediatR(cfg =>
        //{
        //    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        //    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        //});

        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddValidatorsFromAssemblyContaining<Program>();

        services.AddControllers();
        services.AddOpenApi();

        return services;
    }   
}
