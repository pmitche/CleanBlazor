using BlazorHero.CleanArchitecture.Application.Responses.Identity;

namespace BlazorHero.CleanArchitecture.Application.Requests.Identity;

public class UpdateUserRolesRequest
{
    public string UserId { get; set; }
    public IList<UserRoleModel> UserRoles { get; set; }
}
