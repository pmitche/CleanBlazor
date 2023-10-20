using System.Globalization;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Preferences;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Settings;
using BlazorHero.CleanArchitecture.Shared.Constants.Localization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazorHero.CleanArchitecture.Client;

public static class Program
{
    public static async Task Main(string[] args)
    {
        WebAssemblyHostBuilder builder = WebAssemblyHostBuilder
            .CreateDefault(args)
            .AddRootComponents()
            .AddClientServices();
        WebAssemblyHost host = builder.Build();
        var storageService = host.Services.GetRequiredService<ClientPreferenceManager>();
        if (storageService != null)
        {
            CultureInfo culture;
            if (await storageService.GetPreference() is ClientPreference preference)
            {
                culture = new CultureInfo(preference.LanguageCode);
            }
            else
            {
                culture = new CultureInfo(LocalizationConstants.DefaultLanguage.Code);
            }

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }

        await builder.Build().RunAsync();
    }
}
