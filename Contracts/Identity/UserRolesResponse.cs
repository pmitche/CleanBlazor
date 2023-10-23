using BlazorHero.CleanArchitecture.Shared.Models;

namespace BlazorHero.CleanArchitecture.Contracts.Identity;

public class UserRolesResponse
{
    public List<UserRoleModel> UserRoles { get; set; } = new();
}
