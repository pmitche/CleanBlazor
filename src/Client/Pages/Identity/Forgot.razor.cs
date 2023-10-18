using Blazored.FluentValidation;
using BlazorHero.CleanArchitecture.Application.Requests.Identity;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using MudBlazor;

namespace BlazorHero.CleanArchitecture.Client.Pages.Identity;

public partial class Forgot
{
    private readonly ForgotPasswordRequest _emailModel = new();
    private FluentValidationValidator _fluentValidationValidator;
    private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

    private async Task SubmitAsync()
    {
        IResult result = await UserManager.ForgotPasswordAsync(_emailModel);
        if (result.Succeeded)
        {
            SnackBar.Add(Localizer["Done!"], Severity.Success);
            NavigationManager.NavigateTo("/");
        }
        else
        {
            foreach (var message in result.Messages)
            {
                SnackBar.Add(message, Severity.Error);
            }
        }
    }
}
