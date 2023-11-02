using System.Globalization;
using Blazored.LocalStorage;
using BlazorHero.CleanArchitecture.Client.Authentication;
using BlazorHero.CleanArchitecture.Shared.Constants.Permission;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;

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
            .AddAuth()
            .AddHttp();

        builder
            .Services
            .AddLocalization(options => { options.ResourcesPath = Path.Combine("Configuration", "Resources"); })
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
            .AddScoped<AuthenticationStateProvider, BlazorHeroStateProvider>();

        return builder;
    }

    private static WebAssemblyHostBuilder AddAuth(this WebAssemblyHostBuilder builder)
    {
        builder.Services
            .AddTransient<RefreshTokenDelegatingHandler>()
            .AddTransient<AuthenticationHeaderHandler>()
            .AddTransient<AuthenticationManager>()
            .AddAuthorizationCore(options =>
            {
                Permissions.GetRegisteredPermissions().ForEach(permission => options.AddPolicy(permission,
                    policy => policy.RequireClaim(ApplicationClaimTypes.Permission, permission)));
            });

        return builder;
    }

    private static WebAssemblyHostBuilder AddHttp(this WebAssemblyHostBuilder builder)
    {
        builder.Services
            .AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(ClientName))
            .AddHttpClient(ClientName,
                client =>
                {
                    client.DefaultRequestHeaders.AcceptLanguage.Clear();
                    client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture
                        ?.TwoLetterISOLanguageName);
                    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
                })
            .AddHttpMessageHandler<RefreshTokenDelegatingHandler>()
            .AddHttpMessageHandler<AuthenticationHeaderHandler>();

        return builder;
    }
}
