using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Identity.Roles;

public interface IRoleManager : IManager
{
    Task<Result<List<RoleResponse>>> GetRolesAsync();

    Task<Result<string>> SaveAsync(RoleRequest role);

    Task<Result<string>> DeleteAsync(string id);

    Task<Result<PermissionResponse>> GetPermissionsAsync(string roleId);

    Task<Result<string>> UpdatePermissionsAsync(PermissionRequest request);
}
