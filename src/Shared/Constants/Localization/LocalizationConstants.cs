namespace BlazorHero.CleanArchitecture.Shared.Constants.Localization;

public static class LocalizationConstants
{
    public static readonly LanguageCode[] SupportedLanguages =
    {
        new() { Code = "en-US", DisplayName = "English" }, new() { Code = "fr-FR", DisplayName = "French" },
        new() { Code = "km_KH", DisplayName = "Khmer" }, new() { Code = "de-DE", DisplayName = "German" },
        new() { Code = "nl-NL", DisplayName = "Dutch - Netherlands" },
        new() { Code = "es-ES", DisplayName = "Spanish" }, new() { Code = "ru-RU", DisplayName = "Russian" },
        new() { Code = "sv-SE", DisplayName = "Swedish" }, new() { Code = "id-ID", DisplayName = "Indonesia" },
        new() { Code = "it-IT", DisplayName = "Italian" }, new() { Code = "ar", DisplayName = "عربي" }
    };
}
