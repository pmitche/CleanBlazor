using System.Net;
using System.Reflection;
using BlazorHero.CleanArchitecture.Application.Configurations;
using BlazorHero.CleanArchitecture.Application.Features.ExtendedAttributes.Commands.AddEdit;
using BlazorHero.CleanArchitecture.Application.Interfaces.Serialization.Options;
using BlazorHero.CleanArchitecture.Application.Interfaces.Serialization.Serializers;
using BlazorHero.CleanArchitecture.Application.Interfaces.Serialization.Settings;
using BlazorHero.CleanArchitecture.Application.Interfaces.Services;
using BlazorHero.CleanArchitecture.Application.Interfaces.Services.Account;
using BlazorHero.CleanArchitecture.Application.Interfaces.Services.Identity;
using BlazorHero.CleanArchitecture.Application.Serialization.JsonConverters;
using BlazorHero.CleanArchitecture.Application.Serialization.Options;
using BlazorHero.CleanArchitecture.Application.Serialization.Serializers;
using BlazorHero.CleanArchitecture.Application.Serialization.Settings;
using BlazorHero.CleanArchitecture.Infrastructure;
using BlazorHero.CleanArchitecture.Infrastructure.Contexts;
using BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;
using BlazorHero.CleanArchitecture.Infrastructure.Services;
using BlazorHero.CleanArchitecture.Infrastructure.Services.Identity;
using BlazorHero.CleanArchitecture.Infrastructure.Shared.Services;
using BlazorHero.CleanArchitecture.Server.Localization;
using BlazorHero.CleanArchitecture.Server.Services;
using BlazorHero.CleanArchitecture.Shared.Constants.Permission;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi.Models;
using Serilog;

namespace BlazorHero.CleanArchitecture.Server.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddForwarding(this IServiceCollection services, IConfiguration configuration)
    {
        IConfigurationSection applicationSettingsConfiguration = configuration.GetSection(nameof(AppConfiguration));
        var config = applicationSettingsConfiguration.Get<AppConfiguration>();
        if (!config.BehindSslProxy)
        {
            return services;
        }

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            if (string.IsNullOrWhiteSpace(config.ProxyIp))
            {
                return;
            }

            var ipCheck = config.ProxyIp;
            if (IPAddress.TryParse(ipCheck, out IPAddress proxyIp))
            {
                options.KnownProxies.Add(proxyIp);
            }
            else
            {
                Log.Warning("Invalid Proxy IP of {IpCheck}, Not Loaded", ipCheck);
            }
        });

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    builder
                        .AllowCredentials()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithOrigins(config.ApplicationUrl.TrimEnd('/'));
                });
        });

        return services;
    }

    internal static IServiceCollection AddServerLocalization(this IServiceCollection services)
    {
        services.TryAddTransient(typeof(IStringLocalizer<>), typeof(ServerLocalizer<>));
        return services;
    }

    internal static AppConfiguration GetApplicationSettings(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        IConfigurationSection applicationSettingsConfiguration = configuration.GetSection(nameof(AppConfiguration));
        services.Configure<AppConfiguration>(applicationSettingsConfiguration);
        return applicationSettingsConfiguration.Get<AppConfiguration>();
    }

    internal static void RegisterSwagger(this IServiceCollection services) =>
        services.AddSwaggerGen(c =>
        {
            //TODO - Lowercase Swagger Documents
            //c.DocumentFilter<LowercaseDocumentFilter>();
            //Refer - https://gist.github.com/rafalkasa/01d5e3b265e5aa075678e0adfd54e23f

            // include all project's xml comments
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!assembly.IsDynamic)
                {
                    var xmlFile = $"{assembly.GetName().Name}.xml";
                    var xmlPath = Path.Combine(baseDirectory, xmlFile);
                    if (File.Exists(xmlPath))
                    {
                        c.IncludeXmlComments(xmlPath);
                    }
                }
            }

            c.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Version = "v1",
                    Title = "BlazorHero.CleanArchitecture",
                    License = new OpenApiLicense
                    {
                        Name = "MIT License", Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });

            c.AddSecurityDefinition("Bearer",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "Input your Bearer token in this format - Bearer {your token here} to access this API"
                });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                        Scheme = "Bearer",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
        });

    internal static IServiceCollection AddSerialization(this IServiceCollection services)
    {
        services
            .AddScoped<IJsonSerializerOptions, SystemTextJsonOptions>()
            .Configure<SystemTextJsonOptions>(configureOptions =>
            {
                if (configureOptions.JsonSerializerOptions.Converters.All(c =>
                        c.GetType() != typeof(TimespanJsonConverter)))
                {
                    configureOptions.JsonSerializerOptions.Converters.Add(new TimespanJsonConverter());
                }
            });
        services.AddScoped<IJsonSerializerSettings, NewtonsoftJsonSettings>();

        services.AddScoped<IJsonSerializer, SystemTextJsonSerializer>(); // you can change it
        return services;
    }

    internal static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
        => services
            .AddDbContext<BlazorHeroContext>(options => options
                .UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
            .AddTransient<IDatabaseSeeder, DatabaseSeeder>();

    internal static IServiceCollection AddCurrentUserService(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        return services;
    }

    internal static IServiceCollection AddIdentity(this IServiceCollection services)
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

    internal static IServiceCollection AddSharedInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<IDateTimeService, SystemDateTimeService>();
        services.Configure<MailConfiguration>(configuration.GetSection("MailConfiguration"));
        services.AddTransient<IMailService, SmtpMailService>();
        return services;
    }

    internal static IServiceCollection AddFluentValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<AppConfiguration>();
        services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
        return services;
    }

    internal static IServiceCollection AddApplicationServices(this IServiceCollection services)
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

    internal static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        AppConfiguration config)
    {
        services
            .AddAuthentication(authentication =>
            {
                authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer();

        services.ConfigureOptions<ConfigureJwtBearerOptions>();
        services.AddAuthorization(options =>
        {
            Permissions.GetRegisteredPermissions().ForEach(permission => options.AddPolicy(permission,
                policy => policy.RequireClaim(ApplicationClaimTypes.Permission, permission)));
        });
        return services;
    }

    internal static void AddExtendedAttributesValidators(this IServiceCollection services)
    {
        #region AddEditExtendedAttributeCommandValidator

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

        #endregion AddEditExtendedAttributeCommandValidator
    }
}
