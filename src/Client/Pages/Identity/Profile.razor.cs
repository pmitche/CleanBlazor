using System.Net.Http.Json;
using System.Security.Claims;
using Blazored.FluentValidation;
using BlazorHero.CleanArchitecture.Application.Enums;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;
using BlazorHero.CleanArchitecture.Client.Shared.Dialogs;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Shared.Constants.Storage;
using BlazorHero.CleanArchitecture.Shared.Enums;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace BlazorHero.CleanArchitecture.Client.Pages.Identity;

public partial class Profile
{
    private readonly UpdateProfileRequest _profileModel = new();

    private IBrowserFile _file;
    private char _firstLetterOfName;
    private FluentValidationValidator _fluentValidationValidator;
    private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

    private string UserId { get; set; }

    [Parameter] public string ImageDataUrl { get; set; }

    private async Task UpdateProfileAsync()
    {
        var result = await HttpClient.PutAsJsonAsync<UpdateProfileRequest, Result>(
            AccountsEndpoints.UpdateProfile, _profileModel);
        await result.HandleWithSnackBarAsync(SnackBar, async _ =>
        {
            await AuthenticationManager.Logout();
            SnackBar.Success(Localizer["Your Profile has been updated. Please Login to Continue."]);
            NavigationManager.NavigateTo("/");
        });
    }

    protected override async Task OnInitializedAsync() => await LoadDataAsync();

    private async Task LoadDataAsync()
    {
        AuthenticationState state = await StateProvider.GetAuthenticationStateAsync();
        ClaimsPrincipal user = state.User;
        _profileModel.Email = user.GetEmail();
        _profileModel.FirstName = user.GetFirstName();
        _profileModel.LastName = user.GetLastName();
        _profileModel.PhoneNumber = user.GetPhoneNumber();
        UserId = user.GetUserId();
        var result = await HttpClient.GetFromJsonAsync<Result<string>>(AccountsEndpoints.GetProfilePicture(UserId));
        if (result.IsSuccess)
        {
            ImageDataUrl = result.Data;
        }

        if (_profileModel.FirstName.Length > 0)
        {
            _firstLetterOfName = _profileModel.FirstName[0];
        }
    }

    private async Task UploadFiles(InputFileChangeEventArgs e)
    {
        _file = e.File;
        if (_file != null)
        {
            var extension = Path.GetExtension(_file.Name);
            var fileName = $"{UserId}-{Guid.NewGuid()}{extension}";
            const string format = "image/png";
            IBrowserFile imageFile = await e.File.RequestImageFileAsync(format, 400, 400);
            var buffer = new byte[imageFile.Size];
            await imageFile.OpenReadStream().ReadAsync(buffer);
            var request = new UpdateProfilePictureRequest
            {
                Data = buffer, FileName = fileName, Extension = extension, UploadType = UploadType.ProfilePicture
            };
            var result = await HttpClient.PostAsJsonAsync<UpdateProfilePictureRequest, Result<string>>(
                AccountsEndpoints.UpdateProfilePicture(UserId), request);
            await result.HandleWithSnackBarAsync(SnackBar, async (_, base64Image) =>
            {
                await LocalStorage.SetItemAsync(StorageConstants.Local.UserImageUrl, base64Image);
                SnackBar.Success(Localizer["Profile picture added."]);
                NavigationManager.NavigateTo("/account", true);
            });
        }
    }

    private async Task DeleteAsync()
    {
        var parameters = new DialogParameters
        {
            {
                nameof(DeleteConfirmation.ContentText),
                $"{string.Format(Localizer["Do you want to delete the profile picture of {0}"], _profileModel.Email)}?"
            }
        };
        var options = new DialogOptions
        {
            CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true
        };
        IDialogReference dialog =
            await DialogService.ShowAsync<DeleteConfirmation>(Localizer["Delete"], parameters, options);
        DialogResult dialogResult = await dialog.Result;
        if (!dialogResult.Canceled)
        {
            var request = new UpdateProfilePictureRequest
            {
                Data = null, FileName = string.Empty, UploadType = UploadType.ProfilePicture
            };
            var result = await HttpClient.PostAsJsonAsync<UpdateProfilePictureRequest, Result<string>>(
                AccountsEndpoints.UpdateProfilePicture(UserId), request);
            await result.HandleWithSnackBarAsync(SnackBar, async _ =>
            {
                await LocalStorage.RemoveItemAsync(StorageConstants.Local.UserImageUrl);
                ImageDataUrl = string.Empty;
                SnackBar.Success(Localizer["Profile picture deleted."]);
                NavigationManager.NavigateTo("/account", true);
            });
        }
    }
}
