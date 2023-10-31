using BlazorHero.CleanArchitecture.Contracts.Documents;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Misc.DocumentType;

public interface IDocumentTypeManager : IManager
{
    Task<Result<List<GetAllDocumentTypesResponse>>> GetAllAsync();

    Task<Result<int>> SaveAsync(AddEditDocumentTypeRequest request);

    Task<Result<int>> DeleteAsync(int id);

    Task<Result<string>> ExportToExcelAsync(string searchString = "");
}
