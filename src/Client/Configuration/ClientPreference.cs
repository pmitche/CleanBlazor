using BlazorHero.CleanArchitecture.Shared.Constants.Localization;
using BlazorHero.CleanArchitecture.Shared.Settings;

namespace BlazorHero.CleanArchitecture.Client.Configuration;

public record ClientPreference : IPreference
{
    public bool IsDarkMode { get; set; }
    public bool IsRtl { get; set; }
    public bool IsDrawerOpen { get; set; }
    public string PrimaryColor { get; set; }

    public string LanguageCode { get; set; } = LocalizationConstants.DefaultLanguage.Code;
}
