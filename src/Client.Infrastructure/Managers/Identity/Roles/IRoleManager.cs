using BlazorHero.CleanArchitecture.Application.Requests.Identity;
using BlazorHero.CleanArchitecture.Application.Responses.Identity;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Identity.Roles;

public interface IRoleManager : IManager
{
    Task<IResult<List<RoleResponse>>> GetRolesAsync();

    Task<IResult<string>> SaveAsync(RoleRequest role);

    Task<IResult<string>> DeleteAsync(string id);

    Task<IResult<PermissionResponse>> GetPermissionsAsync(string roleId);

    Task<IResult<string>> UpdatePermissionsAsync(PermissionRequest request);
}
