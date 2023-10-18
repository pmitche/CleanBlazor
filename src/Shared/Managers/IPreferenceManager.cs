using BlazorHero.CleanArchitecture.Shared.Settings;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Shared.Managers;

public interface IPreferenceManager
{
    Task SetPreference(IPreference preference);

    Task<IPreference> GetPreference();

    Task<IResult> ChangeLanguageAsync(string languageCode);
}
