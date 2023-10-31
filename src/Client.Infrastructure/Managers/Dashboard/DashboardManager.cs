using BlazorHero.CleanArchitecture.Client.Infrastructure.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;
using BlazorHero.CleanArchitecture.Contracts.Dashboard;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Dashboard;

public class DashboardManager : IDashboardManager
{
    private readonly HttpClient _httpClient;

    public DashboardManager(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<Result<DashboardDataResponse>> GetDataAsync()
    {
        HttpResponseMessage response = await _httpClient.GetAsync(DashboardEndpoints.GetData);
        Result<DashboardDataResponse> data = await response.ToResult<DashboardDataResponse>();
        return data;
    }
}
