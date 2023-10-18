namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;

public static class DocumentTypesEndpoints
{
    public const string Export = "api/documentTypes/export";

    public const string GetAll = "api/documentTypes";
    public const string Delete = "api/documentTypes";
    public const string Save = "api/documentTypes";
    public const string GetCount = "api/documentTypes/count";

    public static string ExportFiltered(string searchString) => $"{Export}?searchString={searchString}";
}
