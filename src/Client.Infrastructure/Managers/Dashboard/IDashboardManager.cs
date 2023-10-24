using BlazorHero.CleanArchitecture.Contracts.Dashboard;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Dashboard;

public interface IDashboardManager : IManager
{
    Task<IResult<DashboardDataResponse>> GetDataAsync();
}
