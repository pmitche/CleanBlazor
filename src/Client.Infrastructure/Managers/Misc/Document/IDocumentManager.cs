using BlazorHero.CleanArchitecture.Contracts.Documents;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Misc.Document;

public interface IDocumentManager : IManager
{
    Task<PaginatedResult<GetAllDocumentsResponse>> GetAllAsync(GetAllPagedDocumentsRequest request);

    Task<IResult<GetDocumentByIdResponse>> GetByIdAsync(int id);

    Task<IResult<int>> SaveAsync(AddEditDocumentRequest request);

    Task<IResult<int>> DeleteAsync(int id);
}
