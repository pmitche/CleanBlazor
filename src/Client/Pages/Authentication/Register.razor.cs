using Blazored.FluentValidation;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Shared.Constants.Routes;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using MudBlazor;

namespace BlazorHero.CleanArchitecture.Client.Pages.Authentication;

public partial class Register
{
    private FluentValidationValidator _fluentValidationValidator;
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

    private bool _passwordVisibility;
    private RegisterRequest _registerUserModel = new();
    private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

    private async Task SubmitAsync() => await HttpClient.PostAsJsonAsync<RegisterRequest, Result>(
            UsersEndpoints.Register, _registerUserModel)
        .Match(message =>
            {
                SnackBar.Success(message);
                NavigationManager.NavigateTo("/login");
                _registerUserModel = new RegisterRequest();
            },
            errors => SnackBar.Error(errors)
        );

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
}
