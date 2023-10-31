using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Identity.RoleClaims;

public interface IRoleClaimManager : IManager
{
    Task<Result<List<RoleClaimResponse>>> GetRoleClaimsAsync();

    Task<Result<List<RoleClaimResponse>>> GetRoleClaimsByRoleIdAsync(string roleId);

    Task<Result<string>> SaveAsync(RoleClaimRequest role);

    Task<Result<string>> DeleteAsync(string id);
}
