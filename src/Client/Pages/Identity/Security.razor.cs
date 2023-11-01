using System.Net.Http.Json;
using Blazored.FluentValidation;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using MudBlazor;

namespace BlazorHero.CleanArchitecture.Client.Pages.Identity;

public partial class Security
{
    private readonly ChangePasswordRequest _passwordModel = new();
    private InputType _currentPasswordInput = InputType.Password;
    private string _currentPasswordInputIcon = Icons.Material.Filled.VisibilityOff;

    private bool _currentPasswordVisibility;
    private FluentValidationValidator _fluentValidationValidator;
    private InputType _newPasswordInput = InputType.Password;
    private string _newPasswordInputIcon = Icons.Material.Filled.VisibilityOff;

    private bool _newPasswordVisibility;
    private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

    private async Task ChangePasswordAsync()
    {
        var result = await HttpClient.PutAsJsonAsync<ChangePasswordRequest, Result>(
            AccountsEndpoints.ChangePassword, _passwordModel);
        result.HandleWithSnackBar(SnackBar, _ =>
        {
            SnackBar.Success(Localizer["Password Changed!"]);
            _passwordModel.Password = string.Empty;
            _passwordModel.NewPassword = string.Empty;
            _passwordModel.ConfirmNewPassword = string.Empty;
        });
    }

    private void TogglePasswordVisibility(bool newPassword)
    {
        if (newPassword)
        {
            if (_newPasswordVisibility)
            {
                _newPasswordVisibility = false;
                _newPasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                _newPasswordInput = InputType.Password;
            }
            else
            {
                _newPasswordVisibility = true;
                _newPasswordInputIcon = Icons.Material.Filled.Visibility;
                _newPasswordInput = InputType.Text;
            }
        }
        else
        {
            if (_currentPasswordVisibility)
            {
                _currentPasswordVisibility = false;
                _currentPasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                _currentPasswordInput = InputType.Password;
            }
            else
            {
                _currentPasswordVisibility = true;
                _currentPasswordInputIcon = Icons.Material.Filled.Visibility;
                _currentPasswordInput = InputType.Text;
            }
        }
    }
}
