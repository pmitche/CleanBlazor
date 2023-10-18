using System.Security.Claims;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Shared.Constants.Storage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorHero.CleanArchitecture.Client.Shared.Components;

public partial class UserCard
{
    [Parameter] public string Class { get; set; }
    private string FirstName { get; set; }
    private string SecondName { get; set; }
    private string Email { get; set; }
    private char FirstLetterOfName { get; set; }

    [Parameter] public string ImageDataUrl { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadDataAsync();
        }
    }

    private async Task LoadDataAsync()
    {
        AuthenticationState state = await StateProvider.GetAuthenticationStateAsync();
        ClaimsPrincipal user = state.User;

        Email = user.GetEmail().Replace(".com", string.Empty);
        FirstName = user.GetFirstName();
        SecondName = user.GetLastName();
        if (FirstName.Length > 0)
        {
            FirstLetterOfName = FirstName[0];
        }

        var imageResponse = await LocalStorage.GetItemAsync<string>(StorageConstants.Local.UserImageUrl);
        if (!string.IsNullOrEmpty(imageResponse))
        {
            ImageDataUrl = imageResponse;
        }

        StateHasChanged();
    }
}
