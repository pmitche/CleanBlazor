using BlazorHero.CleanArchitecture.Contracts.Documents;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Misc.Document;

public interface IDocumentManager : IManager
{
    Task<PaginatedResult<GetAllDocumentsResponse>> GetAllAsync(GetAllPagedDocumentsRequest request);

    Task<Result<GetDocumentByIdResponse>> GetByIdAsync(int id);

    Task<Result<int>> SaveAsync(AddEditDocumentRequest request);

    Task<Result<int>> DeleteAsync(int id);
}
