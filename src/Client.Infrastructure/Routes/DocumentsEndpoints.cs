namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;

public static class DocumentsEndpoints
{
    public const string Save = "api/documents";
    public const string Delete = "api/documents";

    public static string GetAllPaged(int pageNumber, int pageSize, string searchString) =>
        $"api/documents?pageNumber={pageNumber}&pageSize={pageSize}&searchString={searchString}";

    public static string GetById(int documentId) => $"api/documents/{documentId}";
}
