using System.Reflection;
using BlazorHero.CleanArchitecture.Application.Interfaces.Repositories;
using BlazorHero.CleanArchitecture.Application.Interfaces.Serialization.Serializers;
using BlazorHero.CleanArchitecture.Application.Interfaces.Services.Storage;
using BlazorHero.CleanArchitecture.Application.Interfaces.Services.Storage.Provider;
using BlazorHero.CleanArchitecture.Application.Serialization.JsonConverters;
using BlazorHero.CleanArchitecture.Application.Serialization.Options;
using BlazorHero.CleanArchitecture.Application.Serialization.Serializers;
using BlazorHero.CleanArchitecture.Infrastructure.Repositories;
using BlazorHero.CleanArchitecture.Infrastructure.Services.Storage;
using BlazorHero.CleanArchitecture.Infrastructure.Services.Storage.Provider;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorHero.CleanArchitecture.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructureMappings(this IServiceCollection services) =>
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

    public static IServiceCollection AddRepositories(this IServiceCollection services) =>
        services
            .AddTransient(typeof(IRepositoryAsync<,>), typeof(RepositoryAsync<,>))
            .AddTransient<IProductRepository, ProductRepository>()
            .AddTransient<IBrandRepository, BrandRepository>()
            .AddTransient<IDocumentRepository, DocumentRepository>()
            .AddTransient<IDocumentTypeRepository, DocumentTypeRepository>()
            .AddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));

    public static IServiceCollection AddExtendedAttributesUnitOfWork(this IServiceCollection services) =>
        services
            .AddTransient(typeof(IExtendedAttributeUnitOfWork<,,>), typeof(ExtendedAttributeUnitOfWork<,,>));

    public static IServiceCollection AddServerStorage(this IServiceCollection services)
        => AddServerStorage(services, null);

    private static IServiceCollection AddServerStorage(
        this IServiceCollection services,
        Action<SystemTextJsonOptions> configure) =>
        services
            .AddScoped<IJsonSerializer, SystemTextJsonSerializer>()
            .AddScoped<IStorageProvider, ServerStorageProvider>()
            .AddScoped<IServerStorageService, ServerStorageService>()
            .AddScoped<ISyncServerStorageService, ServerStorageService>()
            .Configure<SystemTextJsonOptions>(configureOptions =>
            {
                configure?.Invoke(configureOptions);
                if (!configureOptions.JsonSerializerOptions.Converters.Any(c =>
                        c.GetType() == typeof(TimespanJsonConverter)))
                {
                    configureOptions.JsonSerializerOptions.Converters.Add(new TimespanJsonConverter());
                }
            });
}
