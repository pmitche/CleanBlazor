using BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services.Storage;
using BlazorHero.CleanArchitecture.Server.Configuration;
using BlazorHero.CleanArchitecture.Shared.Constants.Storage;
using BlazorHero.CleanArchitecture.Shared.Settings;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Server.Managers.Preferences;

public class ServerPreferenceManager : IServerPreferenceManager
{
    private readonly IStringLocalizer<ServerPreferenceManager> _localizer;
    private readonly IServerStorageService _serverStorageService;

    public ServerPreferenceManager(
        IServerStorageService serverStorageService,
        IStringLocalizer<ServerPreferenceManager> localizer)
    {
        _serverStorageService = serverStorageService;
        _localizer = localizer;
    }

    public async Task<Result> ChangeLanguageAsync(string languageCode)
    {
        if (await GetPreference() is not ServerPreference preference)
        {
            return Result.Fail(_localizer["Failed to get server preferences"]);
        }

        preference.LanguageCode = languageCode;
        await SetPreference(preference);
        return Result.Ok(_localizer["Server Language has been changed"]);
    }

    public async Task<IPreference> GetPreference() =>
        await _serverStorageService.GetItemAsync<ServerPreference>(StorageConstants.Server.Preference) ??
        new ServerPreference();

    public async Task SetPreference(IPreference preference) =>
        await _serverStorageService.SetItemAsync(StorageConstants.Server.Preference, preference as ServerPreference);
}
