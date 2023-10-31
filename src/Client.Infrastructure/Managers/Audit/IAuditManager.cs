using BlazorHero.CleanArchitecture.Contracts.Audit;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Audit;

public interface IAuditManager : IManager
{
    Task<Result<IEnumerable<AuditResponse>>> GetCurrentUserTrailsAsync();

    Task<Result<string>> DownloadFileAsync(
        string searchString = "",
        bool searchInOldValues = false,
        bool searchInNewValues = false);
}
