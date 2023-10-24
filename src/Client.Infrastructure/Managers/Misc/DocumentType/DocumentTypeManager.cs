using System.Net.Http.Json;
using BlazorHero.CleanArchitecture.Application.Features.DocumentTypes.Commands;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;
using BlazorHero.CleanArchitecture.Contracts.Documents;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Misc.DocumentType;

public class DocumentTypeManager : IDocumentTypeManager
{
    private readonly HttpClient _httpClient;

    public DocumentTypeManager(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<IResult<string>> ExportToExcelAsync(string searchString = "")
    {
        HttpResponseMessage response = await _httpClient.GetAsync(string.IsNullOrWhiteSpace(searchString)
            ? DocumentTypesEndpoints.Export
            : DocumentTypesEndpoints.ExportFiltered(searchString));
        return await response.ToResult<string>();
    }

    public async Task<IResult<int>> DeleteAsync(int id)
    {
        HttpResponseMessage response = await _httpClient.DeleteAsync($"{DocumentTypesEndpoints.Delete}/{id}");
        return await response.ToResult<int>();
    }

    public async Task<IResult<List<GetAllDocumentTypesResponse>>> GetAllAsync()
    {
        HttpResponseMessage response = await _httpClient.GetAsync(DocumentTypesEndpoints.GetAll);
        return await response.ToResult<List<GetAllDocumentTypesResponse>>();
    }

    public async Task<IResult<int>> SaveAsync(AddEditDocumentTypeCommand request)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(DocumentTypesEndpoints.Save, request);
        return await response.ToResult<int>();
    }
}
