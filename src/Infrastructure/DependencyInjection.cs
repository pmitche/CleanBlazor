using System.Reflection;
using BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services;
using BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services.Identity;
using BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services.Storage;
using BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services.Storage.Provider;
using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence;
using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence.Repositories;
using BlazorHero.CleanArchitecture.Infrastructure.Contexts;
using BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;
using BlazorHero.CleanArchitecture.Infrastructure.Repositories;
using BlazorHero.CleanArchitecture.Infrastructure.Services;
using BlazorHero.CleanArchitecture.Infrastructure.Services.Identity;
using BlazorHero.CleanArchitecture.Infrastructure.Services.Storage;
using BlazorHero.CleanArchitecture.Infrastructure.Services.Storage.Provider;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorHero.CleanArchitecture.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDatabase(configuration)
            .AddRepositories()
            .AddIdentity()
            .AddServices()
            .AddInfrastructureMappings()
            .AddServerStorage();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<IRoleClaimService, RoleClaimService>();
        services.AddTransient<ITokenService, IdentityService>();
        services.AddTransient<IRoleService, RoleService>();
        services.AddTransient<IAccountService, AccountService>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IChatService, ChatService>();
        services.AddTransient<IUploadService, UploadService>();
        services.AddTransient<IAuditService, AuditService>();
        services.AddScoped<IExcelService, ExcelService>();
        return services;
    }

    private static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
        => services
            .AddDbContext<BlazorHeroContext>(options => options
                .UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
            .AddTransient<IDatabaseSeeder, DatabaseSeeder>();

    private static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services
            .AddIdentity<BlazorHeroUser, BlazorHeroRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<BlazorHeroContext>()
            .AddDefaultTokenProviders();

        return services;
    }

    private static IServiceCollection AddInfrastructureMappings(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }


    private static IServiceCollection AddRepositories(this IServiceCollection services) =>
        services
            .AddTransient(typeof(IRepositoryAsync<,>), typeof(RepositoryAsync<,>))
            .AddTransient<IProductRepository, ProductRepository>()
            .AddTransient<IBrandRepository, BrandRepository>()
            .AddTransient<IDocumentRepository, DocumentRepository>()
            .AddTransient<IDocumentTypeRepository, DocumentTypeRepository>()
            .AddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));

    private static IServiceCollection AddServerStorage(this IServiceCollection services)
    {
        services
            .AddScoped<IStorageProvider, ServerStorageProvider>()
            .AddScoped<IServerStorageService, ServerStorageService>()
            .AddScoped<ISyncServerStorageService, ServerStorageService>();

        return services;
    }
}
