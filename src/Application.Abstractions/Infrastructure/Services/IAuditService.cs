using BlazorHero.CleanArchitecture.Contracts.Audit;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services;

public interface IAuditService
{
    Task<Result<IEnumerable<AuditResponse>>> GetCurrentUserTrailsAsync(string userId);

    Task<Result<string>> ExportToExcelAsync(
        string userId,
        string searchString = "",
        bool searchInOldValues = false,
        bool searchInNewValues = false);
}
