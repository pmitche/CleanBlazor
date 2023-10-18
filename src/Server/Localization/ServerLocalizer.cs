using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Server.Localization;

internal class ServerLocalizer<T> where T : class
{
    public ServerLocalizer(IStringLocalizer<T> localizer) => Localizer = localizer;

    public IStringLocalizer<T> Localizer { get; }
}
