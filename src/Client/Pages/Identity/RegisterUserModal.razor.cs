using Blazored.FluentValidation;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazorHero.CleanArchitecture.Client.Pages.Identity;

public partial class RegisterUserModal
{
    private readonly RegisterRequest _registerUserModel = new();
    private FluentValidationValidator _fluentValidationValidator;
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

    private bool _passwordVisibility;
    private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

    private void Cancel() => MudDialog.Cancel();

    private async Task SubmitAsync()
    {
        Result response = await UserManager.RegisterUserAsync(_registerUserModel);
        if (response.IsSuccess)
        {
            SnackBar.Success(response.Messages[0]);
            MudDialog.Close();
        }
        else
        {
            SnackBar.Error(response.Messages);
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
}
