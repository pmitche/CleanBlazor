using System.Net.Http.Json;
using System.Security.Claims;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Shared.Constants.Permission;
using BlazorHero.CleanArchitecture.Shared.Models.Identity;
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
        var result = await HttpClient.GetFromJsonAsync<Result<UserResponse>>(UsersEndpoints.GetById(userId));
        if (result.IsSuccess)
        {
            UserResponse user = result.Data;
            if (user != null)
            {
                Title = $"{user.FirstName} {user.LastName}";
                Description = string.Format(Localizer["Manage {0} {1}'s Roles"], user.FirstName, user.LastName);
                var rolesResult = await HttpClient.GetFromJsonAsync<Result<UserRolesResponse>>(
                        UsersEndpoints.GetUserRolesById(user.Id));
                UserRolesList = rolesResult.Data.UserRoles;
            }
        }

        _loaded = true;
    }

    private async Task SaveAsync()
    {
        var request = new UpdateUserRolesRequest { UserId = Id, UserRoles = UserRolesList };
        var result = await HttpClient.PutAsJsonAsync<UpdateUserRolesRequest, Result>(
            UsersEndpoints.GetUserRolesById(request.UserId), request);
        result.HandleWithSnackBar(SnackBar, messages =>
        {
            SnackBar.Success(messages[0]);
            NavigationManager.NavigateTo("/identity/users");
        });
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
