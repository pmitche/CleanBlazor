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

    public async Task<Result<string>> DeleteAsync(string id)
    {
        HttpResponseMessage response = await _httpClient.DeleteAsync(RolesEndpoints.DeleteById(id));
        return await response.ToResult<string>();
    }

    public async Task<Result<List<RoleResponse>>> GetRolesAsync()
    {
        HttpResponseMessage response = await _httpClient.GetAsync(RolesEndpoints.GetAll);
        return await response.ToResult<List<RoleResponse>>();
    }

    public async Task<Result<string>> SaveAsync(RoleRequest role)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(RolesEndpoints.Save, role);
        return await response.ToResult<string>();
    }

    public async Task<Result<PermissionResponse>> GetPermissionsAsync(string roleId)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(RolesEndpoints.GetPermissionsById(roleId));
        return await response.ToResult<PermissionResponse>();
    }

    public async Task<Result<string>> UpdatePermissionsAsync(PermissionRequest request)
    {
        HttpResponseMessage response =
            await _httpClient.PutAsJsonAsync(RolesEndpoints.UpdatePermissionsId(request.RoleId), request);
        return await response.ToResult<string>();
    }
}
