using BlazorHero.CleanArchitecture.Shared.Constants.Localization;
using BlazorHero.CleanArchitecture.Shared.Settings;

namespace BlazorHero.CleanArchitecture.Server.Configuration;

public record ServerPreference : IPreference
{
    public string LanguageCode { get; set; } = LocalizationConstants.DefaultLanguage.Code;

    //TODO - add server preferences
}
