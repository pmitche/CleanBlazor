using System.Net.Http.Json;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Components;

namespace BlazorHero.CleanArchitecture.Client.Pages.Identity;

public partial class UserProfile
{
    private bool _active;
    private string _email;
    private char _firstLetterOfName;
    private string _firstName;
    private string _lastName;

    private bool _loaded;
    private string _phoneNumber;
    [Parameter] public string Id { get; set; }
    [Parameter] public string Title { get; set; }
    [Parameter] public string Description { get; set; }

    [Parameter] public string ImageDataUrl { get; set; }

    private async Task ToggleUserStatus()
    {
        var request = new ToggleUserStatusRequest { ActivateUser = _active, UserId = Id };
        var result = await HttpClient.PostAsJsonAsync<ToggleUserStatusRequest, Result>(
            UsersEndpoints.ToggleUserStatus(request.UserId), request);
        result.HandleWithSnackBar(SnackBar, _ =>
        {
            SnackBar.Success(Localizer["Updated User Status."]);
            NavigationManager.NavigateTo("/identity/users");
        });
    }

    protected override async Task OnInitializedAsync()
    {
        var userId = Id;
        var result = await HttpClient.GetFromJsonAsync<Result<UserResponse>>(UsersEndpoints.GetById(userId));
        if (result.IsSuccess)
        {
            UserResponse user = result.Data;
            if (user != null)
            {
                _firstName = user.FirstName;
                _lastName = user.LastName;
                _email = user.Email;
                _phoneNumber = user.PhoneNumber;
                _active = user.IsActive;
                var profilePictureResult = await HttpClient.GetFromJsonAsync<Result<string>>(
                    AccountsEndpoints.GetProfilePicture(userId));
                if (profilePictureResult.IsSuccess)
                {
                    ImageDataUrl = profilePictureResult.Data;
                }
            }

            Title = $"{_firstName} {_lastName}'s {Localizer["Profile"]}";
            Description = _email;
            if (_firstName.Length > 0)
            {
                _firstLetterOfName = _firstName[0];
            }
        }

        _loaded = true;
    }
}
