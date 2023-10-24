using BlazorHero.CleanArchitecture.Shared.Models;
using BlazorHero.CleanArchitecture.Shared.Models.Identity;

namespace BlazorHero.CleanArchitecture.Contracts.Identity;

public class UserRolesResponse
{
    public List<UserRoleModel> UserRoles { get; set; } = new();
}
