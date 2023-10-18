namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;

public static class BrandsEndpoints
{
    public const string Export = "api/v1/brands/export";

    public const string GetAll = "api/v1/brands";
    public const string Delete = "api/v1/brands";
    public const string Save = "api/v1/brands";
    public const string GetCount = "api/v1/brands/count";
    public const string Import = "api/v1/brands/import";

    public static string ExportFiltered(string searchString) => $"{Export}?searchString={searchString}";
}
