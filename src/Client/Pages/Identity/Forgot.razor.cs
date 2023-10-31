using Blazored.FluentValidation;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Pages.Identity;

public partial class Forgot
{
    private readonly ForgotPasswordRequest _emailModel = new();
    private FluentValidationValidator _fluentValidationValidator;
    private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

    private async Task SubmitAsync()
    {
        Result result = await UserManager.ForgotPasswordAsync(_emailModel);
        if (result.IsSuccess)
        {
            SnackBar.Success(Localizer["Done!"]);
            NavigationManager.NavigateTo("/");
        }
        else
        {
            SnackBar.Error(result.Messages);
        }
    }
}
