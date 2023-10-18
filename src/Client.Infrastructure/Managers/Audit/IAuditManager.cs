using BlazorHero.CleanArchitecture.Application.Responses.Audit;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Audit;

public interface IAuditManager : IManager
{
    Task<IResult<IEnumerable<AuditResponse>>> GetCurrentUserTrailsAsync();

    Task<IResult<string>> DownloadFileAsync(
        string searchString = "",
        bool searchInOldValues = false,
        bool searchInNewValues = false);
}
