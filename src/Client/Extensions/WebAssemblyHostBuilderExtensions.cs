using System.Globalization;
using Blazored.LocalStorage;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Authentication;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Managers;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.ExtendedAttribute;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Preferences;
using BlazorHero.CleanArchitecture.Domain.Entities.ExtendedAttributes;
using BlazorHero.CleanArchitecture.Domain.Entities.Misc;
using BlazorHero.CleanArchitecture.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace BlazorHero.CleanArchitecture.Client.Extensions;

public static class WebAssemblyHostBuilderExtensions
{
    private const string ClientName = "BlazorHero.API";

    public static WebAssemblyHostBuilder AddRootComponents(this WebAssemblyHostBuilder builder)
    {
        builder.RootComponents.Add<App>("#app");

        return builder;
    }

    public static WebAssemblyHostBuilder AddClientServices(this WebAssemblyHostBuilder builder)
    {
        builder
            .Services
            .AddLocalization(options => { options.ResourcesPath = Path.Combine("Configuration", "Resources"); })
            .AddAuthorizationCore(RegisterPermissionClaims)
            .AddBlazoredLocalStorage()
            .AddMudServices(configuration =>
            {
                configuration.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
                configuration.SnackbarConfiguration.HideTransitionDuration = 100;
                configuration.SnackbarConfiguration.ShowTransitionDuration = 100;
                configuration.SnackbarConfiguration.VisibleStateDuration = 3000;
                configuration.SnackbarConfiguration.ShowCloseIcon = false;
            })
            .AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())
            .AddScoped<ClientPreferenceManager>()
            .AddScoped<BlazorHeroStateProvider>()
            .AddScoped<AuthenticationStateProvider, BlazorHeroStateProvider>()
            .AddManagers()
            .AddExtendedAttributeManagers()
            .AddTransient<AuthenticationHeaderHandler>()
            .AddScoped(sp => sp
                .GetRequiredService<IHttpClientFactory>()
                .CreateClient(ClientName).EnableIntercept(sp))
            .AddHttpClient(ClientName,
                client =>
                {
                    client.DefaultRequestHeaders.AcceptLanguage.Clear();
                    client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture
                        ?.TwoLetterISOLanguageName);
                    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
                })
            .AddHttpMessageHandler<AuthenticationHeaderHandler>();
        builder.Services.AddHttpClientInterceptor();
        return builder;
    }

    private static IServiceCollection AddManagers(this IServiceCollection services)
    {
        Type managers = typeof(IManager);

        var types = managers
            .Assembly
            .GetExportedTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Select(t => new { Service = t.GetInterface($"I{t.Name}"), Implementation = t })
            .Where(t => t.Service != null);

        foreach (var type in types.Where(type => managers.IsAssignableFrom(type.Service)))
        {
            services.AddTransient(type.Service, type.Implementation);
        }

        return services;
    }

    private static IServiceCollection AddExtendedAttributeManagers(this IServiceCollection services) =>
        //TODO - add managers with reflection!
        services
            .AddTransient(typeof(IExtendedAttributeManager<int, int, Document, DocumentExtendedAttribute>),
                typeof(ExtendedAttributeManager<int, int, Document, DocumentExtendedAttribute>));

    private static void RegisterPermissionClaims(AuthorizationOptions options) =>
        Permissions.GetRegisteredPermissions().ForEach(permission => options.AddPolicy(permission,
            policy => policy.RequireClaim(ApplicationClaimTypes.Permission, permission)));
}
