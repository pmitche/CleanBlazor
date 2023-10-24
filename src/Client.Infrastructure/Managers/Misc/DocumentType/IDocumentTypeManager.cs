using BlazorHero.CleanArchitecture.Application.Features.DocumentTypes.Commands;
using BlazorHero.CleanArchitecture.Contracts.Documents;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Misc.DocumentType;

public interface IDocumentTypeManager : IManager
{
    Task<IResult<List<GetAllDocumentTypesResponse>>> GetAllAsync();

    Task<IResult<int>> SaveAsync(AddEditDocumentTypeCommand request);

    Task<IResult<int>> DeleteAsync(int id);

    Task<IResult<string>> ExportToExcelAsync(string searchString = "");
}
