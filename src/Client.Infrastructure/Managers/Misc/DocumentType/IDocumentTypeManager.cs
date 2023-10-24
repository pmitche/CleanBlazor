using BlazorHero.CleanArchitecture.Contracts.Documents;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Misc.DocumentType;

public interface IDocumentTypeManager : IManager
{
    Task<IResult<List<GetAllDocumentTypesResponse>>> GetAllAsync();

    Task<IResult<int>> SaveAsync(AddEditDocumentTypeRequest request);

    Task<IResult<int>> DeleteAsync(int id);

    Task<IResult<string>> ExportToExcelAsync(string searchString = "");
}
