using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services.Identity;

public interface IRoleService
{
    Task<Result<List<RoleResponse>>> GetAllAsync();

    Task<int> GetCountAsync();

    Task<Result<RoleResponse>> GetByIdAsync(string id);

    Task<Result<string>> SaveAsync(RoleRequest request);

    Task<Result<string>> DeleteAsync(string id);

    Task<Result<PermissionResponse>> GetAllPermissionsAsync(string roleId);

    Task<Result<string>> UpdatePermissionsAsync(PermissionRequest request);
}
