using BlazorHero.CleanArchitecture.Client.Extensions;
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
        Result result = await UserManager.ToggleUserStatusAsync(request);
        if (result.IsSuccess)
        {
            SnackBar.Success(Localizer["Updated User Status."]);
            NavigationManager.NavigateTo("/identity/users");
        }
        else
        {
            SnackBar.Error(result.Messages);
        }
    }

    protected override async Task OnInitializedAsync()
    {
        var userId = Id;
        Result<UserResponse> result = await UserManager.GetAsync(userId);
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
                Result<string> data = await AccountManager.GetProfilePictureAsync(userId);
                if (data.IsSuccess)
                {
                    ImageDataUrl = data.Data;
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
