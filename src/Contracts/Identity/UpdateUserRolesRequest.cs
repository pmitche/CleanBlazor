using BlazorHero.CleanArchitecture.Shared.Models;
using BlazorHero.CleanArchitecture.Shared.Models.Identity;

namespace BlazorHero.CleanArchitecture.Contracts.Identity;

public class UpdateUserRolesRequest
{
    public string UserId { get; set; }
    public IList<UserRoleModel> UserRoles { get; set; }
}
