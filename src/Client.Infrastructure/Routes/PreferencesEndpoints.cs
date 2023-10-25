namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;

public static class PreferencesEndpoints
{
    private const string Prefix = "api/v1/preferences";

    public const string ChangeLanguage = $"{Prefix}/changeLanguage";
}
