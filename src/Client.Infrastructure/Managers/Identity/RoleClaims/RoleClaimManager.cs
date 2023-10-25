using System.Net.Http.Json;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Identity.RoleClaims;

public class RoleClaimManager : IRoleClaimManager
{
    private readonly HttpClient _httpClient;

    public RoleClaimManager(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<IResult<string>> DeleteAsync(string id)
    {
        HttpResponseMessage response = await _httpClient.DeleteAsync(RoleClaimsEndpoints.DeleteByRoleId(id));
        return await response.ToResult<string>();
    }

    public async Task<IResult<List<RoleClaimResponse>>> GetRoleClaimsAsync()
    {
        HttpResponseMessage response = await _httpClient.GetAsync(RoleClaimsEndpoints.GetAll);
        return await response.ToResult<List<RoleClaimResponse>>();
    }

    public async Task<IResult<List<RoleClaimResponse>>> GetRoleClaimsByRoleIdAsync(string roleId)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(RoleClaimsEndpoints.GetAllByRoleId(roleId));
        return await response.ToResult<List<RoleClaimResponse>>();
    }

    public async Task<IResult<string>> SaveAsync(RoleClaimRequest role)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(RoleClaimsEndpoints.Save, role);
        return await response.ToResult<string>();
    }
}
