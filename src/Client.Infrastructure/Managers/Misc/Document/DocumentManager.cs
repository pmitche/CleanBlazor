using System.Net.Http.Json;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;
using BlazorHero.CleanArchitecture.Contracts.Documents;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Misc.Document;

public class DocumentManager : IDocumentManager
{
    private readonly HttpClient _httpClient;

    public DocumentManager(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<IResult<int>> DeleteAsync(int id)
    {
        HttpResponseMessage response = await _httpClient.DeleteAsync($"{DocumentsEndpoints.Delete}/{id}");
        return await response.ToResult<int>();
    }

    public async Task<PaginatedResult<GetAllDocumentsResponse>> GetAllAsync(GetAllPagedDocumentsRequest request)
    {
        HttpResponseMessage response =
            await _httpClient.GetAsync(DocumentsEndpoints.GetAllPaged(request.PageNumber,
                request.PageSize,
                request.SearchString));
        return await response.ToPaginatedResult<GetAllDocumentsResponse>();
    }

    public async Task<IResult<GetDocumentByIdResponse>> GetByIdAsync(int id)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(DocumentsEndpoints.GetById(id));
        return await response.ToResult<GetDocumentByIdResponse>();
    }

    public async Task<IResult<int>> SaveAsync(AddEditDocumentRequest request)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(DocumentsEndpoints.Save, request);
        return await response.ToResult<int>();
    }
}
