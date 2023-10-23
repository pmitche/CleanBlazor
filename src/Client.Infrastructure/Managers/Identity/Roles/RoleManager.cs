using System.Net.Http.Json;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Identity.Roles;

public class RoleManager : IRoleManager
{
    private readonly HttpClient _httpClient;

    public RoleManager(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<IResult<string>> DeleteAsync(string id)
    {
        HttpResponseMessage response = await _httpClient.DeleteAsync($"{RolesEndpoints.Delete}/{id}");
        return await response.ToResult<string>();
    }

    public async Task<IResult<List<RoleResponse>>> GetRolesAsync()
    {
        HttpResponseMessage response = await _httpClient.GetAsync(RolesEndpoints.GetAll);
        return await response.ToResult<List<RoleResponse>>();
    }

    public async Task<IResult<string>> SaveAsync(RoleRequest role)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(RolesEndpoints.Save, role);
        return await response.ToResult<string>();
    }

    public async Task<IResult<PermissionResponse>> GetPermissionsAsync(string roleId)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(RolesEndpoints.GetPermissions + roleId);
        return await response.ToResult<PermissionResponse>();
    }

    public async Task<IResult<string>> UpdatePermissionsAsync(PermissionRequest request)
    {
        HttpResponseMessage response = await _httpClient.PutAsJsonAsync(RolesEndpoints.UpdatePermissions, request);
        return await response.ToResult<string>();
    }
}
