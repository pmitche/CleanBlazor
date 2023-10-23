using System.Security.Claims;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Shared.Constants.Permission;
using BlazorHero.CleanArchitecture.Shared.Models;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace BlazorHero.CleanArchitecture.Client.Pages.Identity;

public partial class UserRoles
{
    private bool _bordered;
    private bool _canEditUsers;
    private bool _canSearchRoles;

    private ClaimsPrincipal _currentUser;
    private bool _dense;
    private bool _loaded;
    private string _searchString = "";
    private bool _striped = true;

    private UserRoleModel _userRole = new();
    [Parameter] public string Id { get; set; }
    [Parameter] public string Title { get; set; }
    [Parameter] public string Description { get; set; }
    public List<UserRoleModel> UserRolesList { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        _currentUser = await AuthenticationManager.CurrentUser();
        _canEditUsers = (await AuthorizationService.AuthorizeAsync(_currentUser, Permissions.Users.Edit)).Succeeded;
        _canSearchRoles = (await AuthorizationService.AuthorizeAsync(_currentUser, Permissions.Roles.Search))
            .Succeeded;

        var userId = Id;
        IResult<UserResponse> result = await UserManager.GetAsync(userId);
        if (result.Succeeded)
        {
            UserResponse user = result.Data;
            if (user != null)
            {
                Title = $"{user.FirstName} {user.LastName}";
                Description = string.Format(Localizer["Manage {0} {1}'s Roles"], user.FirstName, user.LastName);
                IResult<UserRolesResponse> response = await UserManager.GetRolesAsync(user.Id);
                UserRolesList = response.Data.UserRoles;
            }
        }

        _loaded = true;
    }

    private async Task SaveAsync()
    {
        var request = new UpdateUserRolesRequest { UserId = Id, UserRoles = UserRolesList };
        IResult result = await UserManager.UpdateRolesAsync(request);
        if (result.Succeeded)
        {
            SnackBar.Success(result.Messages[0]);
            NavigationManager.NavigateTo("/identity/users");
        }
        else
        {
            SnackBar.Error(result.Messages);
        }
    }

    private bool Search(UserRoleModel userRole)
    {
        if (string.IsNullOrWhiteSpace(_searchString))
        {
            return true;
        }

        if (userRole.RoleName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (userRole.RoleDescription?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        return false;
    }
}
