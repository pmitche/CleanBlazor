using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorHero.CleanArchitecture.Contracts;

public static class DependencyInjection
{
    public static IServiceCollection AddContracts(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), ServiceLifetime.Transient);

        return services;
    }
}
