using System.Reflection;
using BlazorHero.CleanArchitecture.Application.Behaviors;
using BlazorHero.CleanArchitecture.Application.Configurations;
using BlazorHero.CleanArchitecture.Application.Features.ExtendedAttributes.Commands.AddEdit;
using BlazorHero.CleanArchitecture.Application.Features.ExtendedAttributes.Commands.Delete;
using BlazorHero.CleanArchitecture.Application.Features.ExtendedAttributes.Queries.Export;
using BlazorHero.CleanArchitecture.Application.Features.ExtendedAttributes.Queries.GetAll;
using BlazorHero.CleanArchitecture.Application.Features.ExtendedAttributes.Queries.GetAllByEntityId;
using BlazorHero.CleanArchitecture.Application.Features.ExtendedAttributes.Queries.GetById;
using BlazorHero.CleanArchitecture.Application.Interfaces.Serialization.Options;
using BlazorHero.CleanArchitecture.Application.Interfaces.Serialization.Serializers;
using BlazorHero.CleanArchitecture.Application.Interfaces.Serialization.Settings;
using BlazorHero.CleanArchitecture.Application.Serialization.JsonConverters;
using BlazorHero.CleanArchitecture.Application.Serialization.Options;
using BlazorHero.CleanArchitecture.Application.Serialization.Serializers;
using BlazorHero.CleanArchitecture.Application.Serialization.Settings;
using BlazorHero.CleanArchitecture.Domain.Contracts;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorHero.CleanArchitecture.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddApplicationServices()
            .AddSerialization()
            .AddExtendedAttributes();

        services.AddOptions<AppConfiguration>().Bind(configuration.GetSection(nameof(AppConfiguration)));

        return services;
    }

    private static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();

        services.AddAutoMapper(executingAssembly);
        services.AddValidatorsFromAssembly(executingAssembly, ServiceLifetime.Transient);
        services.AddMediatR(config => config.RegisterServicesFromAssembly(executingAssembly));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        return services;
    }

    private static IServiceCollection AddSerialization(this IServiceCollection services)
    {
        services
            .AddScoped<IJsonSerializerOptions, SystemTextJsonOptions>()
            .AddScoped<IJsonSerializerSettings, NewtonsoftJsonSettings>()
            .AddScoped<IJsonSerializer, SystemTextJsonSerializer>(); // you can change it

        services.Configure<SystemTextJsonOptions>(configureOptions =>
        {
            if (configureOptions.JsonSerializerOptions.Converters.All(c =>
                    c.GetType() != typeof(TimespanJsonConverter)))
            {
                configureOptions.JsonSerializerOptions.Converters.Add(new TimespanJsonConverter());
            }
        });

        return services;
    }

    private static IServiceCollection AddExtendedAttributes(this IServiceCollection services)
    {
        services
            .AddExtendedAttributesHandlers()
            .AddExtendedAttributesValidators();

        return services;
    }

    private static IServiceCollection AddExtendedAttributesHandlers(this IServiceCollection services)
    {
        var extendedAttributeTypes = typeof(IEntity)
            .Assembly
            .GetExportedTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.BaseType?.IsGenericType == true)
            .Select(t => new { BaseGenericType = t.BaseType, CurrentType = t })
            .Where(t => t.BaseGenericType?.GetGenericTypeDefinition() == typeof(AuditableEntityExtendedAttribute<,,>))
            .ToList();

        foreach (var extendedAttributeType in extendedAttributeTypes)
        {
            List<Type> extendedAttributeTypeGenericArguments =
                extendedAttributeType.BaseGenericType.GetGenericArguments().ToList();
            extendedAttributeTypeGenericArguments.Add(extendedAttributeType.CurrentType);

            #region AddEditExtendedAttributeCommandHandler

            Type tRequest =
                typeof(AddEditExtendedAttributeCommand<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments
                    .ToArray());
            Type tResponse = typeof(Result<>).MakeGenericType(extendedAttributeTypeGenericArguments[0]);
            Type serviceType = typeof(IRequestHandler<,>).MakeGenericType(tRequest, tResponse);
            Type implementationType =
                typeof(AddEditExtendedAttributeCommandHandler<,,,>).MakeGenericType(
                    extendedAttributeTypeGenericArguments.ToArray());
            services.AddScoped(serviceType, implementationType);

            #endregion AddEditExtendedAttributeCommandHandler

            #region DeleteExtendedAttributeCommandHandler

            tRequest = typeof(DeleteExtendedAttributeCommand<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments
                .ToArray());
            tResponse = typeof(Result<>).MakeGenericType(extendedAttributeTypeGenericArguments[0]);
            serviceType = typeof(IRequestHandler<,>).MakeGenericType(tRequest, tResponse);
            implementationType =
                typeof(DeleteExtendedAttributeCommandHandler<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments
                    .ToArray());
            services.AddScoped(serviceType, implementationType);

            #endregion DeleteExtendedAttributeCommandHandler

            #region GetAllExtendedAttributesByEntityIdQueryHandler

            tRequest = typeof(GetAllExtendedAttributesByEntityIdQuery<,,,>).MakeGenericType(
                extendedAttributeTypeGenericArguments.ToArray());
            tResponse = typeof(Result<>).MakeGenericType(typeof(List<>).MakeGenericType(
                typeof(GetAllExtendedAttributesByEntityIdResponse<,>).MakeGenericType(
                    extendedAttributeTypeGenericArguments[0],
                    extendedAttributeTypeGenericArguments[1])));
            serviceType = typeof(IRequestHandler<,>).MakeGenericType(tRequest, tResponse);
            implementationType =
                typeof(GetAllExtendedAttributesByEntityIdQueryHandler<,,,>).MakeGenericType(
                    extendedAttributeTypeGenericArguments.ToArray());
            services.AddScoped(serviceType, implementationType);

            #endregion GetAllExtendedAttributesByEntityIdQueryHandler

            #region GetExtendedAttributeByIdQueryHandler

            tRequest = typeof(GetExtendedAttributeByIdQuery<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments
                .ToArray());
            tResponse = typeof(Result<>).MakeGenericType(
                typeof(GetExtendedAttributeByIdResponse<,>).MakeGenericType(
                    extendedAttributeTypeGenericArguments[0],
                    extendedAttributeTypeGenericArguments[1]));
            serviceType = typeof(IRequestHandler<,>).MakeGenericType(tRequest, tResponse);
            implementationType =
                typeof(GetExtendedAttributeByIdQueryHandler<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments
                    .ToArray());
            services.AddScoped(serviceType, implementationType);

            #endregion GetExtendedAttributeByIdQueryHandler

            #region GetAllExtendedAttributesQueryHandler

            tRequest = typeof(GetAllExtendedAttributesQuery<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments
                .ToArray());
            tResponse = typeof(Result<>).MakeGenericType(typeof(List<>).MakeGenericType(
                typeof(GetAllExtendedAttributesResponse<,>).MakeGenericType(
                    extendedAttributeTypeGenericArguments[0],
                    extendedAttributeTypeGenericArguments[1])));
            serviceType = typeof(IRequestHandler<,>).MakeGenericType(tRequest, tResponse);
            implementationType =
                typeof(GetAllExtendedAttributesQueryHandler<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments
                    .ToArray());
            services.AddScoped(serviceType, implementationType);

            #endregion GetAllExtendedAttributesQueryHandler

            #region ExportExtendedAttributesQueryHandler

            tRequest = typeof(ExportExtendedAttributesQuery<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments
                .ToArray());
            tResponse = typeof(Result<>).MakeGenericType(typeof(string));
            serviceType = typeof(IRequestHandler<,>).MakeGenericType(tRequest, tResponse);
            implementationType =
                typeof(ExportExtendedAttributesQueryHandler<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments
                    .ToArray());
            services.AddScoped(serviceType, implementationType);

            #endregion ExportExtendedAttributesQueryHandler
        }

        return services;
    }

    private static IServiceCollection AddExtendedAttributesValidators(this IServiceCollection services)
    {
        Type addEditExtendedAttributeCommandValidatorType = typeof(AddEditExtendedAttributeCommandValidator<,,,>);
        var validatorTypes = addEditExtendedAttributeCommandValidatorType
            .Assembly
            .GetExportedTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.BaseType?.IsGenericType == true)
            .Select(t => new { BaseGenericType = t.BaseType, CurrentType = t })
            .Where(t => t.BaseGenericType?.GetGenericTypeDefinition() ==
                        typeof(AddEditExtendedAttributeCommandValidator<,,,>))
            .ToList();

        foreach (var validatorType in validatorTypes)
        {
            Type addEditExtendedAttributeCommandType =
                typeof(AddEditExtendedAttributeCommand<,,,>).MakeGenericType(validatorType.BaseGenericType
                    .GetGenericArguments());
            Type iValidator = typeof(IValidator<>).MakeGenericType(addEditExtendedAttributeCommandType);
            services.AddScoped(iValidator, validatorType.CurrentType);
        }

        return services;
    }
}
