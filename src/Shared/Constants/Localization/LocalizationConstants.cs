namespace BlazorHero.CleanArchitecture.Shared.Constants.Localization;

public static class LocalizationConstants
{
    public static readonly LanguageCode[] SupportedLanguages =
    {
        new() { Code = "en-US", DisplayName = "English" },
        new() { Code = "fr-FR", DisplayName = "French" }
    };
}
