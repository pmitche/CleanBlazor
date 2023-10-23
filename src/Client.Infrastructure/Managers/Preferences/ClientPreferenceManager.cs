using Blazored.LocalStorage;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Configuration;
using BlazorHero.CleanArchitecture.Shared.Constants.Storage;
using BlazorHero.CleanArchitecture.Shared.Settings;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Preferences;

public class ClientPreferenceManager : IClientPreferenceManager
{
    private readonly IStringLocalizer<ClientPreferenceManager> _localizer;
    private readonly ILocalStorageService _localStorageService;

    public ClientPreferenceManager(
        ILocalStorageService localStorageService,
        IStringLocalizer<ClientPreferenceManager> localizer)
    {
        _localStorageService = localStorageService;
        _localizer = localizer;
    }

    public async Task<bool> ToggleDarkModeAsync()
    {
        if (await GetPreference() is not ClientPreference preference)
        {
            return false;
        }

        preference.IsDarkMode = !preference.IsDarkMode;
        await SetPreference(preference);
        return !preference.IsDarkMode;
    }

    public async Task<IResult> ChangeLanguageAsync(string languageCode)
    {
        if (await GetPreference() is not ClientPreference preference)
        {
            return new Result
            {
                Succeeded = false, Messages = new List<string> { _localizer["Failed to get client preferences"] }
            };
        }

        preference.LanguageCode = languageCode;
        await SetPreference(preference);
        return new Result
        {
            Succeeded = true, Messages = new List<string> { _localizer["Client Language has been changed"] }
        };

    }

    public async Task<MudTheme> GetCurrentThemeAsync()
    {
        if (await GetPreference() is not ClientPreference preference)
        {
            return BlazorHeroTheme.DefaultTheme;
        }

        return preference.IsDarkMode ? BlazorHeroTheme.DarkTheme : BlazorHeroTheme.DefaultTheme;
    }

    public async Task<IPreference> GetPreference() =>
        await _localStorageService.GetItemAsync<ClientPreference>(StorageConstants.Local.Preference) ??
        new ClientPreference();

    public async Task SetPreference(IPreference preference) =>
        await _localStorageService.SetItemAsync(StorageConstants.Local.Preference, preference as ClientPreference);

    public async Task<bool> ToggleLayoutDirection()
    {
        if (await GetPreference() is not ClientPreference preference)
        {
            return false;
        }

        preference.IsRtl = !preference.IsRtl;
        await SetPreference(preference);
        return preference.IsRtl;

    }

    public async Task<bool> IsRtl()
    {
        var preference = await GetPreference() as ClientPreference;
        return preference is not { IsDarkMode: true } && preference.IsRtl;
    }
}
