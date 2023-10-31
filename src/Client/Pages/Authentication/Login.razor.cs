using System.Security.Claims;
using Blazored.FluentValidation;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace BlazorHero.CleanArchitecture.Client.Pages.Authentication;

public partial class Login
{
    private FluentValidationValidator _fluentValidationValidator;
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

    private bool _passwordVisibility;
    private TokenRequest _tokenModel = new();
    private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

    protected override async Task OnInitializedAsync()
    {
        AuthenticationState state = await StateProvider.GetAuthenticationStateAsync();
        if (state != new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())))
        {
            NavigationManager.NavigateTo("/");
        }
    }

    private async Task SubmitAsync()
    {
        Result result = await AuthenticationManager.Login(_tokenModel);
        if (result.IsFailure)
        {
            SnackBar.Error(result.Messages);
        }
    }

    private void TogglePasswordVisibility()
    {
        if (_passwordVisibility)
        {
            _passwordVisibility = false;
            _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
            _passwordInput = InputType.Password;
        }
        else
        {
            _passwordVisibility = true;
            _passwordInputIcon = Icons.Material.Filled.Visibility;
            _passwordInput = InputType.Text;
        }
    }

    private void FillAdministratorCredentials()
    {
        _tokenModel.Email = "mukesh@blazorhero.com";
        _tokenModel.Password = "123Pa$$word!";
    }

    private void FillBasicUserCredentials()
    {
        _tokenModel.Email = "john@blazorhero.com";
        _tokenModel.Password = "123Pa$$word!";
    }
}
