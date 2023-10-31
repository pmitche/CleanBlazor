using System.Net.Http.Json;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;
using BlazorHero.CleanArchitecture.Contracts.Documents;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Misc.DocumentType;

public class DocumentTypeManager : IDocumentTypeManager
{
    private readonly HttpClient _httpClient;

    public DocumentTypeManager(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<Result<string>> ExportToExcelAsync(string searchString = "")
    {
        HttpResponseMessage response = await _httpClient.GetAsync(string.IsNullOrWhiteSpace(searchString)
            ? DocumentTypesEndpoints.Export
            : DocumentTypesEndpoints.ExportFiltered(searchString));
        return await response.ToResult<string>();
    }

    public async Task<Result<int>> DeleteAsync(int id)
    {
        HttpResponseMessage response = await _httpClient.DeleteAsync(DocumentTypesEndpoints.DeleteById(id));
        return await response.ToResult<int>();
    }

    public async Task<Result<List<GetAllDocumentTypesResponse>>> GetAllAsync()
    {
        HttpResponseMessage response = await _httpClient.GetAsync(DocumentTypesEndpoints.GetAll);
        return await response.ToResult<List<GetAllDocumentTypesResponse>>();
    }

    public async Task<Result<int>> SaveAsync(AddEditDocumentTypeRequest request)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(DocumentTypesEndpoints.Save, request);
        return await response.ToResult<int>();
    }
}
