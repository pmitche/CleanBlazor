using System.Security.Claims;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using BlazorHero.CleanArchitecture.Shared.Constants.Permission;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.JSInterop;
using MudBlazor;

namespace BlazorHero.CleanArchitecture.Client.Pages.Identity;

public partial class Users
{
    private bool _bordered;
    private bool _canCreateUsers;
    private bool _canExportUsers;
    private bool _canSearchUsers;
    private bool _canViewRoles;

    private ClaimsPrincipal _currentUser;
    private bool _dense;
    private bool _loaded;
    private string _searchString = "";
    private bool _striped = true;
    private UserResponse _user = new();
    private List<UserResponse> _userList = new();

    protected override async Task OnInitializedAsync()
    {
        _currentUser = await AuthenticationManager.CurrentUser();
        _canCreateUsers = (await AuthorizationService.AuthorizeAsync(_currentUser, Permissions.Users.Create))
            .Succeeded;
        _canSearchUsers = (await AuthorizationService.AuthorizeAsync(_currentUser, Permissions.Users.Search))
            .Succeeded;
        _canExportUsers = (await AuthorizationService.AuthorizeAsync(_currentUser, Permissions.Users.Export))
            .Succeeded;
        _canViewRoles = (await AuthorizationService.AuthorizeAsync(_currentUser, Permissions.Roles.View)).Succeeded;

        await GetUsersAsync();
        _loaded = true;
    }

    private async Task GetUsersAsync()
    {
        IResult<List<UserResponse>> response = await UserManager.GetAllAsync();
        if (response.Succeeded)
        {
            _userList = response.Data.ToList();
        }
        else
        {
            SnackBar.Error(response.Messages);
        }
    }

    private bool Search(UserResponse user)
    {
        if (string.IsNullOrWhiteSpace(_searchString))
        {
            return true;
        }

        if (user.FirstName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (user.LastName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (user.Email?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (user.PhoneNumber?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (user.UserName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        return false;
    }

    private async Task ExportToExcel()
    {
        var base64 = await UserManager.ExportToExcelAsync(_searchString);
        await JsRuntime.InvokeVoidAsync("Download",
            new
            {
                ByteArray = base64,
                FileName = $"{nameof(Users).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                MimeType = ApplicationConstants.MimeTypes.OpenXml
            });
        SnackBar.Success(string.IsNullOrWhiteSpace(_searchString)
                ? Localizer["Users exported"]
                : Localizer["Filtered Users exported"]);
    }

    private async Task InvokeModal()
    {
        var parameters = new DialogParameters();
        var options = new DialogOptions
        {
            CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true
        };
        IDialogReference dialog =
            await DialogService.ShowAsync<RegisterUserModal>(Localizer["Register New User"], parameters, options);
        DialogResult result = await dialog.Result;
        if (!result.Canceled)
        {
            await GetUsersAsync();
        }
    }

    private void ViewProfile(string userId) => NavigationManager.NavigateTo($"/user-profile/{userId}");

    private void ManageRoles(string userId, string email)
    {
        if (email == "mukesh@blazorhero.com")
        {
            SnackBar.Error(Localizer["Not Allowed."]);
        }
        else
        {
            NavigationManager.NavigateTo($"/identity/user-roles/{userId}");
        }
    }
}
