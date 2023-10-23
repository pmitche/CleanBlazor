using BlazorHero.CleanArchitecture.Contracts.Audit;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Application.Interfaces.Services;

public interface IAuditService
{
    Task<IResult<IEnumerable<AuditResponse>>> GetCurrentUserTrailsAsync(string userId);

    Task<IResult<string>> ExportToExcelAsync(
        string userId,
        string searchString = "",
        bool searchInOldValues = false,
        bool searchInNewValues = false);
}
