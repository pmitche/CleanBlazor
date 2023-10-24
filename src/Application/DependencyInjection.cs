using System.Reflection;
using BlazorHero.CleanArchitecture.Application.Abstractions.Serialization;
using BlazorHero.CleanArchitecture.Application.Behaviors;
using BlazorHero.CleanArchitecture.Application.Configuration;
using BlazorHero.CleanArchitecture.Application.Serialization;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorHero.CleanArchitecture.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();

        services
            .AddAutoMapper(executingAssembly)
            .AddMediatR(config => config.RegisterServicesFromAssembly(executingAssembly))
            .AddValidatorsFromAssembly(executingAssembly, ServiceLifetime.Transient) // TODO: Remove
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
            .AddScoped<IJsonSerializer, SystemTextJsonSerializer>(); // you can change it

        services.AddOptions<AppConfiguration>().Bind(configuration.GetSection(nameof(AppConfiguration)));

        return services;
    }
}
