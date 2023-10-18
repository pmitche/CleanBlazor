namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;

public static class AuditEndpoints
{
    public const string GetCurrentUserTrails = "api/audits";
    public const string DownloadFile = "api/audits/export";

    public static string DownloadFileFiltered(
        string searchString,
        bool searchInOldValues = false,
        bool searchInNewValues = false) =>
        $"{DownloadFile}?searchString={searchString}&searchInOldValues={searchInOldValues}&searchInNewValues={searchInNewValues}";
}
