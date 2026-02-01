using FluentValidation;
using GoldenBread.Application.Interfaces;
using GoldenBread.Application.UseCases;
using GoldenBread.Application.Validators;
using Microsoft.Extensions.DependencyInjection;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace GoldenBread.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Services
        services.AddScoped<IAccountUseCase, AccountUseCase>();

        // Validators
        services.AddValidatorsFromAssemblyContaining<CompanyValidator>();

        return services;
    }
}
